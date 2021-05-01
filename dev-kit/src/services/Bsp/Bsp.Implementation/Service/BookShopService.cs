using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Service.Host;
using Core.Service.Host.Convention.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bsp.Implementation.Service
{
    public partial class BookShopService : StatelessService
    {
        private readonly ILogger<BookShopService> _logger;

        public BookShopService(IOptions<ServiceConfig> config, ILogger<BookShopService> logger)
            : base(config.Value)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //TODO: listen service queue here:
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}