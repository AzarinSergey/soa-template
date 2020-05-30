using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Service.Host.ServiceDiscovery
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, string healthCheckPath)
        {
            var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger("Consul.AspNetCore");

            logger.LogInformation($"Heath check endpoint on $'{healthCheckPath}' ");

            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var consulConfig = app.ApplicationServices.GetRequiredService<IOptions<ConsulConfig>>();
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            var uri = new Uri(consulConfig.Value.ServiceAddress);
            var dnsIpAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                .First(o => o.AddressFamily == AddressFamily.InterNetwork).ToString();

            var registrationModel = new AgentServiceRegistration
            {
                ID = $"{consulConfig.Value.ServiceId}-{uri.Port}",
                Name = consulConfig.Value.ServiceName,
                Address = dnsIpAddress,
                Port = uri.Port,
                Tags = consulConfig.Value.Tags,
                Check = new AgentServiceCheck
                {
                    Timeout = TimeSpan.FromSeconds(3),
                    Interval = TimeSpan.FromSeconds(10),
                    HTTP = $"{uri.Scheme}://{dnsIpAddress}:{uri.Port}{healthCheckPath}",
                    Status = HealthStatus.Critical
                }
            };

            logger.LogInformation("Registering with Consul");
            consulClient.Agent.ServiceDeregister(registrationModel.ID).ConfigureAwait(true);
            consulClient.Agent.ServiceRegister(registrationModel).ConfigureAwait(true);

            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Unregistering from Consul");
                consulClient.Agent.ServiceDeregister(registrationModel.ID).ConfigureAwait(true);
            });

            return app;
        }
    }
}