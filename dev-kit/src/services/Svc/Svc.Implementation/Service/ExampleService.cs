using Core.Service.Host.ServiceDiscovery;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Service.Host;

namespace Svc.Implementation.Service
{
    public partial class ExampleService : StatelessService
    {
        public ILogger<ExampleService> Logger;

        public ExampleService(IOptions<ServiceConfig> config, ILogger<ExampleService> logger)
        : base(config.Value)
        {
            Logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //TODO: слушать сервисую очередь тут
            while (!stoppingToken.IsCancellationRequested)
            {
                Logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
