using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using UltimateRedditBot.App.Factories.GuildFactory;
using UltimateRedditBot.App.Factories.GuildSettingsFactory;
using UltimateRedditBot.App.Factories.QueueFactory;
using UltimateRedditBot.App.Factories.SubRedditFactory;
using UltimateRedditBot.Core.AppService;
using UltimateRedditBot.Core.Repository;
using UltimateRedditBot.Core.Services;
using UltimateRedditBot.Core.Uow;
using UltimateRedditBot.Discord;
using UltimateRedditBot.Discord.Commands;
using UltimateRedditBot.Discord.Modules;
using UltimateRedditBot.Infra.AppServices;
using UltimateRedditBot.Infra.Repository;
using UltimateRedditBot.Infra.Services;
using UltimateRedditBot.Infra.Uow;

namespace UltimateRedditBot.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddUltimateRedditBot(this IServiceCollection services)
        {
            services.AddDiscord();
            services.AddRepository();
            services.AddAppServices();
            services.AddFactories();
            services.AddUltimateServices();
            
        }

        private static void AddDiscord(this IServiceCollection services)
        {
            services.AddSingleton(new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async,
            }))
            .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {                                       // Add discord to the collection
                LogLevel = LogSeverity.Critical,     // Tell the logger to give Verbose amount of info
                MessageCacheSize = 100             // Cache 1,000 messages per channel
            }))
            .AddSingleton<StartDiscord>()
            .AddSingleton<CommandHandler>()
            .AddSingleton<BotModule>()
            .AddSingleton<SettingsModule>();
        }

        private static void AddRepository(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IRepository<,>), typeof(Repository<,>))
                    .AddSingleton(typeof(IRepository<>), typeof(Repository<>));
        }

        private static void AddAppServices(this IServiceCollection services)
        {
            services.AddSingleton<IGuildAppService, GuildAppService>()
                    .AddSingleton<ISubRedditAppService, SubRedditAppService>()
                    .AddSingleton<IPostAppService, PostAppService>()
                    .AddSingleton<ISubRedditHistoryAppService, SubRedditHistoryAppService>();
        }

        private static void AddFactories(this IServiceCollection services)
        {
            services.AddSingleton<IGuildFactory, GuildFactory>()
                    .AddSingleton<ISubRedditFactory, SubRedditFactory>()
                    .AddSingleton<IQueueFactory, QueueFactory>()
                    .AddSingleton<IGuildSettingsFactory, GuildSettingsFactory>();
        }

        private static void AddUltimateServices(this IServiceCollection services)
        {
            services
                    .AddSingleton<IRedditApiService, RedditApiService>()
                    .AddSingleton<IUnitOfWork, UnitOfWork>()
                    .AddSingleton<IQueueService, QueueService>()
                    .AddSingleton<IRedisCacheManager, RedisCacheManager>();

        }
    }
}
