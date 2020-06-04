using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Core.Service.Host.ServiceDiscovery
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseConsul<T>(this IApplicationBuilder app, 
            T consulClient, string healthCheckPath, Type[] serviceEndpointInterfaces)
            where T : IConsulClient
        {
            var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger("Consul.AspNetCore");

            logger.LogInformation($"Heath check endpoint on $'{healthCheckPath}' ");

            var consulConfig = app.ApplicationServices.GetRequiredService<IOptions<ServiceConfig>>();
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

            logger.LogInformation("Registering service with Consul");

            consulClient.Agent.ServiceDeregister(registrationModel.ID).ConfigureAwait(true);
            consulClient.Agent.ServiceRegister(registrationModel).ConfigureAwait(true);

            foreach (var type in serviceEndpointInterfaces)
            {
                //TODO: вынести логику получения имени сервисного интерфейса для построения пути вызова
                if (!type.IsInterface)
                    throw new ArgumentException($"Interface type only allowed to Consul registration.");

                var serviceInterfaceName = type.Name.StartsWith("I")
                    ? new string(type.Name.Skip(1).ToArray())
                    : type.Name;

                consulClient.KV.Put(new KVPair(type.FullName)
                {
                    Value = Encoding.UTF8.GetBytes($"{consulConfig.Value.ServiceName}/{serviceInterfaceName}")
                });
            }

            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Unregistering from Consul");
                consulClient.Agent.ServiceDeregister(registrationModel.ID).ConfigureAwait(true);

                foreach (var type in serviceEndpointInterfaces)
                {
                    consulClient.KV.Delete(type.FullName);
                }
            });

            return app;
        }

        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, 
            string healthCheckPath, Type[] serviceEndpointInterfaces)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            return app.UseConsul(consulClient, healthCheckPath, serviceEndpointInterfaces);
        }
    }
}