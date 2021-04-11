using System;
using Core.Service.Host.Client.DynamicProxy;
using Core.Service.Host.Convention.Configuration;
using Core.Service.Host.Convention.Convention;
using Core.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Core.Service.Host.Client.ServiceCollectionExtensions
{
    public static class InternalServiceRegistrator
    {
        public static void RegisterInternalServiceProxy<T>(this IServiceCollection services, string serviceName)
            where T : IInternalHttpService
        {
            services.AddHttpClient<ServiceProxy<T>>((provider, client) =>
            {
                var cfg = provider.GetRequiredService<IOptions<ApplicationConfig>>().Value;
                var convention = provider.GetRequiredService<IServiceEndpointConvention>();
                client.BaseAddress = new Uri($"http://{serviceName}:");
            });
        }
    }
}