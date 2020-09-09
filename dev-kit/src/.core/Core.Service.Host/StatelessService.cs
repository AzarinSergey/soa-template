using Core.Service.Host.ServiceDiscovery.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Core.Service.Host
{
    public abstract class StatelessService : BackgroundService, IInternalHttpService
    {
        protected StatelessService(IServiceConfig config)
        {
            Config = config;
        }

        protected readonly IServiceConfig Config;
    }
}