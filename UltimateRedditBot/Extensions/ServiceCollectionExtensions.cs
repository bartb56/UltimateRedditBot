using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using UltimateRedditBot.App.Factories.ChannelFactory;
using UltimateRedditBot.App.Factories.GuildFactory;
using UltimateRedditBot.App.Factories.GuildSettingsFactory;
using UltimateRedditBot.App.Factories.QueueFactory;
using UltimateRedditBot.App.Factories.SubRedditFactory;
using UltimateRedditBot.App.Factories.SubRedditHistoryFactory;
using UltimateRedditBot.App.Factories.Subscriptions;
using UltimateRedditBot.Core.AppService;
using UltimateRedditBot.Core.Repository;
using UltimateRedditBot.Core.Services;
using UltimateRedditBot.Discord;
using UltimateRedditBot.Discord.Commands;
using UltimateRedditBot.Discord.Modules;
using UltimateRedditBot.Infra.AppServices;
using UltimateRedditBot.Infra.Repository;
using UltimateRedditBot.Infra.Services;

namespace UltimateRedditBot.Extensions
{
    public static class ServiceCollectionExtensions
    {
        #region Methods

        /// <summary>
        /// Registers all the services.
        /// </summary>
        /// <param name="services"></param>
        public static void AddUltimateRedditBot(this IServiceCollection services)
        {
            services.AddDiscord();
            services.AddRepository();
            services.AddAppServices();
            services.AddFactories();
            services.AddUltimateServices();
        }

        /// <summary>
        /// Add all the services required for the discord bot.
        /// </summary>
        /// <param name="services"></param>
        private static void AddDiscord(this IServiceCollection services)
        {
            services.AddSingleton(new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async
            }))
            .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Critical,
                MessageCacheSize = 100
            }))
            .AddSingleton<StartDiscord>()
            .AddSingleton<CommandHandler>()
            .AddSingleton<RedditModule>()
            .AddSingleton<SettingsModule>();
        }

        /// <summary>
        /// Add the base repository and unit of work.
        /// </summary>
        /// <param name="services"></param>
        private static void AddRepository(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IRepository<,>), typeof(Repository<,>))
                    .AddSingleton(typeof(IRepository<>), typeof(Repository<>));
        }

        /// <summary>
        /// Register all the app services.
        /// </summary>
        /// <param name="services"></param>
        private static void AddAppServices(this IServiceCollection services)
        {
            services.AddSingleton<IGuildAppService, GuildAppService>()
                .AddSingleton<ISubRedditAppService, SubRedditAppService>()
                .AddSingleton<IPostAppService, PostAppService>()
                .AddSingleton<ISubscriptionAppService, SubscriptionAppService>()
                .AddSingleton<IChannelSubscriptionMapperAppService, ChannelSubscriptionMapperAppService>()
                .AddSingleton<IGuildSettingsAppService, GuildSettingsAppService>()
                .AddSingleton<ISubRedditHistoryAppService, SubRedditHistoryAppService>();
        }

        /// <summary>
        /// Register the factories
        /// </summary>
        /// <param name="services"></param>
        private static void AddFactories(this IServiceCollection services)
        {
            services.AddSingleton<IGuildFactory, GuildFactory>()
                    .AddSingleton<ISubRedditFactory, SubRedditFactory>()
                    .AddSingleton<IQueueFactory, QueueFactory>()
                    .AddSingleton<IGuildSettingsFactory, GuildSettingsFactory>()
                    .AddSingleton<IChannelFactory, ChannelFactory>()
                    .AddSingleton<ISubRedditHistoryFactory, SubRedditHistoryFactory>()
                    .AddSingleton<ISubscriptionsFactory, SubscriptionsFactory>();
        }

        /// <summary>
        /// Register the services.
        /// </summary>
        /// <param name="services"></param>
        private static void AddUltimateServices(this IServiceCollection services)
        {
            services
                    .AddSingleton<IRedditApiService, RedditApiService>()
                    .AddSingleton<ISubscriptionService, SubscriptionService>()
                    .AddSingleton<IQueueService, QueueService>()
                    .AddSingleton<IChannelAppService, ChannelAppService>()
                    .AddSingleton<IRedisCacheManager, RedisCacheManager>();

        }

        #endregion
    }
}
