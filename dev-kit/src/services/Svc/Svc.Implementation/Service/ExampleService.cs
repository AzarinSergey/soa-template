using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Svc.Implementation.Service
{
    public partial class ExampleService : BackgroundService
    {
        public ILogger<ExampleService> Logger;

        public ExampleService(ILogger<ExampleService> logger)
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
