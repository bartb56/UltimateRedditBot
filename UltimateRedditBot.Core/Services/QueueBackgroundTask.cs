using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using UltimateRedditBot.Infra.Services;

namespace UltimateRedditBot.Core.Services
{
    public class QueueBackgroundTask : BackgroundService
    {
        private readonly IQueueService _queueService;

        public QueueBackgroundTask(IQueueService queueService)
        {
            _queueService = queueService;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        await _queueService.ProcessQueue();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    await Task.Delay(1000, stoppingToken);
                }, stoppingToken);
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            ExecuteAsync(cancellationToken);
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
