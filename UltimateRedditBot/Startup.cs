﻿using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using UltimateRedditBot.Core.Services;
using UltimateRedditBot.Database;
using UltimateRedditBot.Discord;
using UltimateRedditBot.Discord.Commands;
using UltimateRedditBot.Extensions;

namespace UltimateRedditBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //Set the configuration

        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddSerilog());
            services.AddHttpClient();

            //Register the dbContext
            AddBotDbContext(services);

            //Register all the ultimate reddit bot services.s
            services.AddUltimateRedditBot();

            //Add the background tasks.
            services.AddHostedService<SubscriptionBackgroundTask>();

            var provider = services.BuildServiceProvider();

            //Run the bot startup service.
            provider.GetRequiredService<StartDiscord>().StartAsync().Wait();
            provider.GetRequiredService<CommandHandler>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging();
        }

        private void AddBotDbContext(IServiceCollection services)
        {
            services.AddSingleton(Configuration)
                    .AddDbContext<Context>(options =>
                    {
                        options.UseSqlServer(Configuration["ConnectionString:DefaultConnection"]);
                    });
        }
    }
}
