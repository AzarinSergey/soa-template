using Core.Service.Host.ServiceDiscovery;
using Core.Service.Host.ServiceDiscovery.Interfaces;
using Core.Service.Host.ServiceDiscovery.Provider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Service.Host
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServiceDiscovery(this IServiceCollection services, IConfiguration configuration)
        {
            var configSection = configuration.GetSection(typeof(ServiceDiscoveryConfig).Name).Get<ServiceDiscoveryConfig>();
            var serviceDiscovery = new ServiceDiscoveryConsulProvider(configSection);

            services.AddSingleton<IServiceDiscoveryProvider, ServiceDiscoveryConsulProvider>(p => serviceDiscovery);
            services.AddSingleton<IServiceEndpointConvention, ServiceDiscoveryConsulProvider>(p => serviceDiscovery);

            return services;
        }
    }
}
