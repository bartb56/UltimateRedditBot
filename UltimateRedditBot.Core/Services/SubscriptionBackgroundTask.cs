using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using UltimateRedditBot.Infra.Services;

namespace UltimateRedditBot.Core.Services
{
    public class SubscriptionBackgroundTask : BackgroundService
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionBackgroundTask(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        await _subscriptionService.HandleSubscriptions();
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
#pragma warning disable 4014
            ExecuteAsync(cancellationToken);
#pragma warning restore 4014
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
