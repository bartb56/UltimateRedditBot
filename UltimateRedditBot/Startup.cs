using Discord;
using Discord.Commands;
using Discord.WebSocket;
using EasyCaching.Core.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using UltimateRedditBot.App.Extensions;
using UltimateRedditBot.App.Factories.GuildFactory;
using UltimateRedditBot.App.Factories.GuildSettingsFactory;
using UltimateRedditBot.App.Factories.QueueFactory;
using UltimateRedditBot.App.Factories.SubRedditFactory;
using UltimateRedditBot.Core.AppService;
using UltimateRedditBot.Core.Constants;
using UltimateRedditBot.Core.Repository;
using UltimateRedditBot.Core.Services;
using UltimateRedditBot.Core.Uow;
using UltimateRedditBot.Database;
using UltimateRedditBot.Discord;
using UltimateRedditBot.Discord.Commands;
using UltimateRedditBot.Discord.Modules;
using UltimateRedditBot.Extensions;
using UltimateRedditBot.Infra.AppServices;
using UltimateRedditBot.Infra.Repository;
using UltimateRedditBot.Infra.Services;
using UltimateRedditBot.Infra.Uow;

namespace UltimateRedditBot
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(string[] args)
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
        }

        public static async Task RunAsync(string[] args)
        {
            var startup = new Startup(args);
            await startup.RunAsync();
        }

        public async Task RunAsync()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
           

            var provider = services.BuildServiceProvider();
            var applicationBuilder = new ApplicationBuilder(provider);
            Configure(applicationBuilder);

            //Run the bot startup service.
            await provider.GetRequiredService<StartDiscord>().StartAsync();
            provider.GetRequiredService<CommandHandler>();
            //Keep alive
            await Task.Delay(-1);
        }


        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration)
                .AddDbContext<Context>(options =>
                {
                    options.UseSqlServer(Configuration["ConnectionString:DefaultConnection"]);
                });

            services.AddEasyCaching(options =>
            {
                options.UseRedis(redisConfig =>
                {
                    redisConfig.DBConfig.Endpoints.Add(new ServerEndPoint(Configuration["RedisConnection"], Int32.Parse(Configuration["RedisPort"])));

                    redisConfig.DBConfig.AllowAdmin = true;
                },
                CachingConstants.RedisName);
            });

            services.AddUltimateRedditBot();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseUltimateExceptionHandler();
        }
    }
}
