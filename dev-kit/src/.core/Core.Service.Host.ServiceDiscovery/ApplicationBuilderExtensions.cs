using Core.Service.Host.ServiceDiscovery.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Core.Service.Host.ServiceDiscovery
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseServiceDiscovery(this IApplicationBuilder app,
            string healthCheckPath, Type[] serviceEndpointInterfaces)
        {
            var serviceDiscoveryProvider = app.ApplicationServices.GetRequiredService<IServiceDiscoveryProvider>();
            return app.UseServiceDiscovery(serviceDiscoveryProvider, healthCheckPath, serviceEndpointInterfaces);
        }

        public static IApplicationBuilder UseServiceDiscovery(this IApplicationBuilder app,
            IServiceDiscoveryProvider serviceDiscoveryProvider,
            string healthCheckPath, 
            Type[] serviceEndpointInterfaces)
        {
            var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger(serviceDiscoveryProvider.GetType().FullName);

            logger.LogInformation("Registering service in discovery system...");
            logger.LogInformation($"Heath check endpoint on $'{healthCheckPath}' ");

            serviceDiscoveryProvider.RegisterService(healthCheckPath);
            serviceDiscoveryProvider.AddEndpoints(serviceEndpointInterfaces);

            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Unregistering from discovery system");
                serviceDiscoveryProvider.UnregisterService();
                serviceDiscoveryProvider.RemoveEndpointPrefixes(serviceEndpointInterfaces);
            });

            return app;
        }
    }
}