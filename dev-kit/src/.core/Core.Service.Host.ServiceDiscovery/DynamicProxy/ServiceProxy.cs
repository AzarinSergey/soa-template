using System;
using System.Net.Http;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Core.Service.Host.ServiceDiscovery.DynamicProxy.Http;
using Core.Service.Host.ServiceDiscovery.Interfaces;

namespace Core.Service.Host.ServiceDiscovery.DynamicProxy
{
    public static class ServiceProxy
    {
        private static string _reverseProxyAddress;
        private static IHttpClientFactory _httpClientFactory;
        private static IServiceDiscoveryProvider _serviceDiscoveryProvider;

        private static bool _initialized;
        private static IServiceEndpointConvention _convention;

        public static void Initialization(
            string reverseProxyAddress,
            IHttpClientFactory factory,
            IServiceDiscoveryProvider serviceDiscoveryProvider,
            IServiceEndpointConvention convention)
        {
            _reverseProxyAddress = string.IsNullOrEmpty(reverseProxyAddress) ? throw new ArgumentNullException(nameof(reverseProxyAddress)) : reverseProxyAddress;
            _httpClientFactory = factory ?? throw new ArgumentNullException(nameof(factory));
            _serviceDiscoveryProvider = serviceDiscoveryProvider ?? throw new ArgumentNullException(nameof(serviceDiscoveryProvider));
            _convention = convention ?? throw new ArgumentNullException(nameof(convention));

            _initialized = true;
        }

        public static TService Create<TService>()
            where TService : IDiscoverableHttpService
        {
            if(!_initialized)
                throw new TypeInitializationException(
                    typeof(ServiceProxy).FullName, 
                    new NullReferenceException("Initialization method must be called."));

            var serviceInterfaceType = typeof(TService);
            if (!serviceInterfaceType.IsInterface)
                throw new ArgumentException($"'TService' - interface type only allowed.");

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_reverseProxyAddress);

            var callProcessor = new HttpServiceCallBuilder(
                new HttpServiceDynamicInstance(httpClient),
                serviceInterfaceType,
                _serviceDiscoveryProvider);

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