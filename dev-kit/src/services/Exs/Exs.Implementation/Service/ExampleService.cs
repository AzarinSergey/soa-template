﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Service.Host;
using Core.Service.Host.Convention.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Exs.Implementation.Service
{
    public partial class ExampleService : StatelessService
    {
        private readonly ILogger<ExampleService> _logger;

        public ExampleService(IOptions<ServiceConfig> config, ILogger<ExampleService> logger)
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
