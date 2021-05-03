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

                //Here env var defined when Kubebridge runs at least once for service witch using this http client
                var serviceHost = Environment.GetEnvironmentVariable(hostEnvName);

                var serviceUrl = string.IsNullOrEmpty(serviceHost) 
                    ? $"http://{serviceName}"  
                    : $"http://{serviceHost}";

                client.BaseAddress = new Uri(serviceUrl);
            });
        }
    }
}