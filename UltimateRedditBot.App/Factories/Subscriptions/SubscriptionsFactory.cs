using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using UltimateRedditBot.App.Factories.ChannelFactory;
using UltimateRedditBot.App.Factories.SubRedditFactory;
using UltimateRedditBot.Domain.Enums;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.AppServices;

namespace UltimateRedditBot.App.Factories.Subscriptions
{
    public class SubscriptionsFactory : ISubscriptionsFactory
    {
        #region Fields

        private readonly ISubRedditFactory _subRedditFactory;
        private readonly IChannelFactory _channelFactory;
        private readonly DiscordSocketClient _discord;
        private readonly ISubscriptionAppService _subscriptionAppService;
        private readonly IChannelSubscriptionMapperAppService _subscriptionMapperAppService;

        #endregion

        #region Constructor

        public SubscriptionsFactory(ISubRedditFactory subRedditFactory,
            IChannelFactory channelFactory,
            DiscordSocketClient discord, ISubscriptionAppService subscriptionAppService, IChannelSubscriptionMapperAppService subscriptionMapperAppService)
        {
            _subRedditFactory = subRedditFactory;
            _channelFactory = channelFactory;
            _discord = discord;
            _subscriptionAppService = subscriptionAppService;
            _subscriptionMapperAppService = subscriptionMapperAppService;
        }

        #endregion

        #region Methods

        public async Task<IEnumerable<Subscription>> Subscriptions(ulong channelId)
        {
            var mapper = await _subscriptionMapperAppService.GetByChannelId(channelId);
            return mapper.Select(map => map.Subscription);
        }

        public async Task Subscribe(ulong channelId, string subRedditName, Sort sort)
        {
            var subReddit = await _subRedditFactory.GetSubRedditByName(subRedditName);

            if(!(_discord.GetChannel(channelId) is ITextChannel textChannel))
                return;

            //Validation
            if (subReddit is null)
            {
                await textChannel.SendMessageAsync("The requested subreddit doesn't exist");
                return;
            }
            if (subReddit.IsNsfw && !textChannel.IsNsfw)
                await textChannel.SendMessageAsync("You can only subscribe to this subreddit in nsfw channels.");

            await _channelFactory.AddIfNotExisting(channelId);

            var subscription = await _subscriptionAppService.AddIfNotExisting(subReddit.Id, sort);

            if (await IsAlreadySubscribed(subscription.Id, channelId))
            {
                await textChannel.SendMessageAsync("This channel is already subscribed to the subreddit");
                return;
            }

            await _subscriptionMapperAppService.Insert(new ChannelSubscriptionMapper(channelId, subscription.Id));

            await textChannel.SendMessageAsync("Successfully subscribed");
        }

        public async Task Unsubscribe(ulong channelId, string subRedditName)
        {
            var subReddit = await _subRedditFactory.GetSubRedditByName(subRedditName);

            if(!(_discord.GetChannel(channelId) is ITextChannel textChannel))
                return;

            if (subReddit is null)
            {
                await textChannel.SendMessageAsync("The requested subreddit doesn't exist");
                return;
            }

            var subscription = await _subscriptionAppService.GetBySubRedditAndChannelId(subReddit.Id, channelId);
            if (subscription is null)
                return;

            await _subscriptionMapperAppService.Unsubscribe(channelId, subscription.Id);

            await textChannel.SendMessageAsync("Successfully unsubscribed");
        }

        private async Task<bool> IsAlreadySubscribed(int subscriptionId, ulong channelId)
        {
            return await _subscriptionMapperAppService.GetByChannelIdAndSubscriptionId(channelId, subscriptionId) is not null;
        }

        #endregion


    }
}
