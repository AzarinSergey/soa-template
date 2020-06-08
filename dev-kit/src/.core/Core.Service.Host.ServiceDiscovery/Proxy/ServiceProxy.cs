using System;
using System.Net.Http;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Consul;
using Core.Service.Host.ServiceDiscovery.Interfaces;
using Core.Service.Host.ServiceDiscovery.Proxy.Http;

namespace Core.Service.Host.ServiceDiscovery.Proxy
{
    public static class ServiceProxy
    {
        private static string _reverseProxyAddress;
        private static IHttpClientFactory _httpClientFactory;
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

            var callProcessor = new HttpServiceCallBuilder(
                new HttpServiceDynamicInstance(httpClient),
                serviceType,
                _serviceDiscoveryClient.KV);

            return (TService)new ProxyGenerator().CreateInterfaceProxyWithoutTarget(
                typeof(TService), new HttpInterfaceInterceptor(callProcessor));
        }

        private class HttpInterfaceInterceptor : IInterceptor
        {
            private readonly IHttpServiceCallBuilder _builder;

            public HttpInterfaceInterceptor(IHttpServiceCallBuilder builder)
            {
                _builder = builder;
            }

            public void Intercept(IInvocation invocation)
            {
                var returnType = invocation.Method.ReturnType;
                _builder.AddInvocation(invocation);
                if (returnType == typeof(Task))
                {
                    invocation.ReturnValue = _builder.BuildAsync();
                    return;
                }

                if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var resultType = invocation.Method.ReturnType.GetGenericArguments()[0];
                    var mi = _builder.ProcessAsyncWithResultMethodInfo.MakeGenericMethod(resultType);
                    invocation.ReturnValue = mi.Invoke(_builder, null);
                    return;
                }

                throw new NotSupportedException($"Return type '{returnType}' of method '{invocation.Method.Name}' not supported. 'Task' and 'Task<>' types are supported only.");
            }
        }
    }
}