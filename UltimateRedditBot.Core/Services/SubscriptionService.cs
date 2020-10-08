using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.AppServices;
using UltimateRedditBot.Infra.Services;

namespace UltimateRedditBot.Core.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        #region Fields

        private readonly IRedditApiService _redditApiService;
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly ISubscriptionAppService _subscriptionAppService;

        #endregion

        #region Constructor

        public SubscriptionService(DiscordSocketClient discordSocketClient, IRedditApiService redditApiService, ISubscriptionAppService subscriptionAppService)
        {
            _discordSocketClient = discordSocketClient;
            _redditApiService = redditApiService;
            _subscriptionAppService = subscriptionAppService;
        }


        #endregion

        #region Methods

        public async Task HandleSubscriptions()
        {
            var allSubscriptions = await _subscriptionAppService.GetAllIncluding();

            var subscriptions = allSubscriptions.ToList();
            if (!subscriptions.Any())
                return;

            var updatedSubscriptions = await GetSubscriptionPosts(subscriptions);

            if(updatedSubscriptions is not null )
                await _subscriptionAppService.UpdateRange(updatedSubscriptions);
        }

        private async Task<IEnumerable<Subscription>> GetSubscriptionPosts(IEnumerable<Subscription> subscriptions)
        {
            var postRequests = subscriptions.Select(GetSubscriptionPost).ToList();

            var result = await Task.WhenAll(postRequests);
            return result.Where(subscription => subscription is not null);
        }

        private async Task<Subscription> GetSubscriptionPost(Subscription subscription)
        {
            var post = await _redditApiService.GetNewPost(subscription.SubReddit.Name, subscription.Sort);
            if (post is null)
                return null;

            if (post.Id == subscription.LastPostName)
                return null;

            var channels = subscription.ChannelSubscriptionMappers
                .Where(x => _discordSocketClient.GetChannel(x.ChannelId) is ITextChannel)
                .Select(x => _discordSocketClient.GetChannel(x.ChannelId) as ITextChannel).ToList();

            var messageRequests = channels.Select(async channel =>
            {
                await channel.SendMessageAsync("", false, post.Embed(subscription.SubReddit.Name));
            });

            await Task.WhenAll(messageRequests);
            subscription.UpdateLastPostName(post.Id);

            return subscription;
        }

        #endregion

    }
}
