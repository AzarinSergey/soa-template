using Core.Service.Host.Convention.Configuration;
using Core.Service.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Core.Service.Host
{
    public abstract class StatelessService : BackgroundService, IInternalHttpService
    {
        protected StatelessService(ServiceConfig config)
        {
            Config = config;
        }

        protected readonly ServiceConfig Config;
    }
}