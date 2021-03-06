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
