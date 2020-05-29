using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Api.Gw.ServiceDiscovery
{
    public static class ServiceCollectionExtensions
    {
        private static string _host;

        public static IServiceCollection AddConsul(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConsulConfig>(configuration.GetSection("ServiceConfig"));
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var host = configuration["ServiceConfig:serviceDiscoveryAddress"];
                _host = host;
                consulConfig.Address = new Uri(host);
            }));
            return services;
        }

        public static IApplicationBuilder UseConsul(this IApplicationBuilder app)
        {
            const string healthCheckPath = "/tool/health/ping";
            app.UseRouter(builder =>
            {
                builder.MapGet(healthCheckPath, context => context.Response.WriteAsync("OK"));
            });

            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var consulConfig = app.ApplicationServices.GetRequiredService<IOptions<ConsulConfig>>();
            var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger("Consul.AspNetCore");
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            var uri = new Uri(consulConfig.Value.ServiceAddress);
            var dnsIpAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                .First(o => o.AddressFamily == AddressFamily.InterNetwork).ToString();

            var registration = new AgentServiceRegistration()
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
            consulClient.Agent.ServiceDeregister(registration.ID).ConfigureAwait(true);
            consulClient.Agent.ServiceRegister(registration).ConfigureAwait(true);

            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Unregistering from Consul");
                consulClient.Agent.ServiceDeregister(registration.ID).ConfigureAwait(true);
            });

            return app;
        }
    }
}
