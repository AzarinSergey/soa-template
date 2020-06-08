using System;
using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Service.Host.ServiceDiscovery
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConsul(this IServiceCollection services, IConfiguration configuration)
        {
            var sectionKey = typeof(ServiceDiscoveryConfig).Name;
            services.Configure<ServiceDiscoveryConfig>(configuration.GetSection(sectionKey));
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var host = configuration[$"{sectionKey}:serviceDiscoveryAddress"];
                consulConfig.Address = new Uri(host);
            }));
            return services;
        }
    }
}
