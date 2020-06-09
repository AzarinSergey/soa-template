using Castle.DynamicProxy;
using Core.Service.Host.ServiceDiscovery.Proxy.Http.Content;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

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

        private readonly Func<Type, CancellationToken, Task<string>> _getServicePathPrefix;
        private readonly IHttpServiceDynamicInstance _dynamicProxy;
        private readonly Type _serviceType;
        private IInvocation _invocation;

        private HttpContent _requestContent;
        private CancellationToken _cancellationToken;


        public HttpServiceCallBuilder(
            IHttpServiceDynamicInstance dynamicProxy,
            Type serviceType,
            Func<Type, CancellationToken, Task<string>> getServicePathPrefix)
        {
            _dynamicProxy = dynamicProxy;
            _serviceType = serviceType;
            _getServicePathPrefix = getServicePathPrefix;
        }

        public IHttpServiceCallBuilder AddInvocation(IInvocation invocation)
        {
            //invocation
            _invocation = invocation;

            //token
            var tokenObj = invocation.Arguments.FirstOrDefault(x => x is CancellationToken);
            if (tokenObj == null) _cancellationToken = CancellationToken.None;
            else _cancellationToken = (CancellationToken) tokenObj;

            //content
            var requestBuilder = new DefaultRequestContentBuilder(_serviceType, invocation.Method);
            _requestContent = requestBuilder.BuildContent(_invocation.Arguments);

            //next
            //TODO: get request meta (headers, method type, content type) from service interface type attributes

            return this;
        }

        public async Task BuildAsync()
        {
            using (_requestContent)
                await _dynamicProxy.CallAsync(await GetFullPath(), _requestContent, _cancellationToken);
        }

        public MethodInfo ProcessAsyncWithResultMethodInfo => HandleAsyncMethodInfo;
        private async Task<T> BuildAsyncWithResult<T>()
        {
            using (_requestContent)
                return await _dynamicProxy.CallAsync<T>(await GetFullPath(), _requestContent, _cancellationToken);
        }

        private async Task<string> GetFullPath()
        {
            return $"{await _getServicePathPrefix(_serviceType, CancellationToken.None)}/{_invocation.Method.Name}";
        }
    }
}