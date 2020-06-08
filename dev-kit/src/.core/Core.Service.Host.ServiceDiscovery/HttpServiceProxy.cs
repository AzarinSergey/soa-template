using Castle.DynamicProxy;
using Consul;
using Core.Service.Host.ServiceDiscovery.Interfaces;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service.Host.ServiceDiscovery
{
    public static class ServiceProxy
    {
        private static string _reverseProxyAddress;
        private static IHttpClientFactory _httpClientFactory;
        //TODO: разделить на IKvStorageClient и IServiceDiscoveryClient
        private static IConsulClient _serviceDiscoveryClient;

        private static bool _initialized;

        public static void Initialization(string reverseProxyAddress, IHttpClientFactory factory, IConsulClient client)
        {
            _reverseProxyAddress = string.IsNullOrEmpty(reverseProxyAddress) ? throw new ArgumentNullException(nameof(reverseProxyAddress)) : reverseProxyAddress;
            _httpClientFactory = factory ?? throw new ArgumentNullException(nameof(factory));
            _serviceDiscoveryClient = client ?? throw new ArgumentNullException(nameof(client));

            _initialized = true;
        }

        public static TService Create<TService>()
            where TService : IDiscoverableHttpService
        {
            if(!_initialized)
                throw new TypeInitializationException(
                    typeof(ServiceProxy).FullName, 
                    new NullReferenceException("Initialization method must be called."));

            var serviceType = typeof(TService);
            if (!serviceType.IsInterface)
                throw new ArgumentException($"'TService' - interface type only allowed.");

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_reverseProxyAddress);

            return (TService)new ProxyGenerator().CreateInterfaceProxyWithoutTarget(
                typeof(TService), new HttpServiceCallInterceptor(httpClient, _serviceDiscoveryClient.KV, serviceType));
        }
    }

    public class HttpServiceCallInterceptor : IInterceptor
    {
        private readonly HttpClient _httpClient;
        private readonly IKVEndpoint _kv;
        private readonly string _serviceKey;

        public HttpServiceCallInterceptor(HttpClient httpClient, IKVEndpoint kv, Type serviceType)
        {
            _httpClient = httpClient;
            _kv = kv;
            _serviceKey = serviceType.FullName;
        }
        public void Intercept(IInvocation invocation)
        {
            var parameters = invocation.Method.GetParameters();
            var arguments = invocation.Arguments;
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("{");
            for (int i = 0; i < arguments.Length; i++)
            {
                var propertyName = parameters[i].Name;
                var propertyValue = System.Text.Json.JsonSerializer.Serialize(arguments[i]);

                stringBuilder.Append($"\"{propertyName}\":{propertyValue}");
                stringBuilder.Append(",");
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append("}");

            var jsonBody = stringBuilder.ToString();
            var returnType = invocation.Method.ReturnType;
            if (returnType == typeof(Task))
            {
                invocation.ReturnValue = HandleAsync(invocation.Method.Name, jsonBody);
                return;
            }

            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                ExecuteHandleAsyncWithResultUsingReflection(invocation, jsonBody);
                return;
            }

            throw new NotSupportedException("Return type '' of method '' not supported. 'Task' and 'Task<>' types are supported only.");
        }

        private async Task HandleAsync(string methodName, string jsonContent)
        {
            var path = await GetCallPath(_kv, methodName);
            using var request = new HttpRequestMessage(HttpMethod.Post, path) {Content = new StringContent(jsonContent)};

            await _httpClient.SendAsync(request);
        }

        private async Task<T> HandleAsyncWithResult<T>(string methodName, string jsonContent)
        {
             var path = await GetCallPath(_kv, methodName);
            using var request = new HttpRequestMessage(HttpMethod.Post, path) {Content = new StringContent(jsonContent)};

            var result = await _httpClient.SendAsync(request);

            return System.Text.Json.JsonSerializer.Deserialize<T>(await result.Content.ReadAsStringAsync());
        }

        private async Task<string> GetCallPath(IKVEndpoint kv, string methodName)
        {
            //TODO: memoise
            var getPair = await kv.Get(_serviceKey);

            return $"{Encoding.UTF8.GetString(getPair.Response.Value, 0, getPair.Response.Value.Length)}/{methodName}";
        }

        private static readonly MethodInfo HandleAsyncMethodInfo = typeof(HttpServiceCallInterceptor).GetMethod("HandleAsyncWithResult", BindingFlags.Instance | BindingFlags.NonPublic);

        private void ExecuteHandleAsyncWithResultUsingReflection(IInvocation invocation, string jsonContent)
        {
            var resultType = invocation.Method.ReturnType.GetGenericArguments()[0];
            var mi = HandleAsyncMethodInfo.MakeGenericMethod(resultType);
            invocation.ReturnValue = mi.Invoke(this, new object[] { invocation.Method.Name, jsonContent });
        }
    }
}