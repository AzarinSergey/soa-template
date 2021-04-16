using Core.Service.Host.Client.DynamicProxy;
using Core.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Core.Service.Host.Client.ServiceCollectionExtensions
{
    public static class InternalServiceRegistrator
    {
        public static void RegisterInternalServiceProxy<T>(this IServiceCollection services, string serviceName)
            where T : IInternalHttpService
        {
            services.AddHttpClient<ServiceProxy<T>>((provider, client) =>
            {
                var hostEnvName = serviceName.ToUpperInvariant() + "_SERVICE_HOST";
                var serviceHost = Environment.GetEnvironmentVariable(hostEnvName);

                client.BaseAddress = new Uri($"http://{serviceHost}");
            });
        }
    }
}