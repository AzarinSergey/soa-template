using System;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Consul;
using Core.Service.Host.ServiceDiscovery.Proxy.Http.Content;

namespace Core.Service.Host.ServiceDiscovery.Proxy.Http
{
    internal interface IInvocationBuilder
    {
        IHttpServiceCallBuilder AddInvocation(IInvocation invocation);
    }
    internal interface IHttpServiceCallBuilder : IInvocationBuilder
    {
        MethodInfo ProcessAsyncWithResultMethodInfo { get; }
        Task BuildAsync();
    }

    internal class HttpServiceCallBuilder : IHttpServiceCallBuilder
    {
        private static readonly MethodInfo HandleAsyncMethodInfo = typeof(HttpServiceCallBuilder)
            .GetMethod("BuildAsyncWithResult", BindingFlags.Instance | BindingFlags.NonPublic);

        private readonly IHttpServiceDynamicInstance _dynamicProxy;
        private readonly Type _serviceType;
        private readonly IKVEndpoint _servicePathStorage;
        private IInvocation _invocation;
        private DefaultRequestContentBuilder _requestBuilder;
        private HttpContent _requestContent;

        public HttpServiceCallBuilder(IHttpServiceDynamicInstance dynamicProxy, Type serviceType, IKVEndpoint servicePathStorage)
        {
            _dynamicProxy = dynamicProxy;
            _serviceType = serviceType;
            _servicePathStorage = servicePathStorage;
        }

        public IHttpServiceCallBuilder AddInvocation(IInvocation invocation)
        {
            _invocation = invocation;
         
            //TODO: separate build func
            _requestBuilder = new DefaultRequestContentBuilder(_serviceType, invocation.Method);
            _requestContent = _requestBuilder.BuildContent(_invocation.Arguments);

            //TODO: get request meta (headers, method type, content type) from service interface type attributes

            return this;
        }

        public async Task BuildAsync()
        {
            using (_requestContent)
            {
                await _dynamicProxy.CallAsync(await GetPath(), _requestContent);
            }
        }

        public MethodInfo ProcessAsyncWithResultMethodInfo => HandleAsyncMethodInfo;
        private async Task<T> BuildAsyncWithResult<T>()
        {
            using (_requestContent)
            {
                return await _dynamicProxy.CallAsync<T>(await GetPath(), _requestContent);
            }
        }

        private async Task<string> GetPath()
        {
            var getPair = await _servicePathStorage.Get(_serviceType.FullName);

            return $"{Encoding.UTF8.GetString(getPair.Response.Value, 0, getPair.Response.Value.Length)}/{_invocation.Method.Name}";
        }
    }
}