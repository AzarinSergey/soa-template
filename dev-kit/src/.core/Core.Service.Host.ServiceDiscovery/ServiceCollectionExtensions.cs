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
            services.Configure<ServiceConfig>(configuration.GetSection("ServiceConfig"));
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var host = configuration["ServiceConfig:serviceDiscoveryAddress"];
                consulConfig.Address = new Uri(host);
            }));
            return services;
        }

        
    }
}
