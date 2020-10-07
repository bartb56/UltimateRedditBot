using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UltimateRedditBot.App.Constants;
using UltimateRedditBot.App.Factories.SubRedditFactory;
using UltimateRedditBot.Domain.Enums;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Domain.Queue;
using UltimateRedditBot.Infra.AppServices;
using UltimateRedditBot.Infra.Services;

namespace UltimateRedditBot.App.Factories.QueueFactory
{
    public class QueueFactory : IQueueFactory
    {
        #region Fields

        private readonly ISubRedditFactory _subRedditFactory;
        private readonly DiscordSocketClient _discord;
        private readonly IGuildSettingsAppService _guildSettingsAppService;
        private readonly IQueueManagerService _queueManagerService;

        private static readonly SemaphoreSlim LazyAddToQueue = new SemaphoreSlim(1, 1);

        #endregion

        #region Constructor

        public QueueFactory(ISubRedditFactory subRedditFactory, DiscordSocketClient discord, IGuildSettingsAppService guildSettingsAppService, IQueueManagerService queueManagerService)
        {
            _subRedditFactory = subRedditFactory;
            _discord = discord;
            _guildSettingsAppService = guildSettingsAppService;
            _queueManagerService = queueManagerService;
        }

        #endregion

        #region Methods

        public async Task AddToQueue(ulong guildId, string subRedditName, PostType post, ulong channelId, int amountOfTimes)
        {
            await LazyAddToQueue.WaitAsync();
            try
            {
                var txtChannel = _discord.GetChannel(channelId) as ITextChannel;
                if (txtChannel is null)
                    return; //Channel has been removed.

                var subReddit = await GetSubReddit(subRedditName);
                if (subReddit is null)
                {
                    await txtChannel.SendMessageAsync("The subreddit could not be found.");
                    LazyAddToQueue.Release();
                    return;
                }

                if (subReddit.IsNsfw && !txtChannel.IsNsfw)
                {
                    await txtChannel.SendMessageAsync("This subreddit can only be used in nsfw channels.");
                    LazyAddToQueue.Release();
                    return;
                }

                var sort = await _guildSettingsAppService.GetGuildSettingByKey<Sort>(guildId,
                    DefaultSettingKeys.Sort);

                IEnumerable<QueueItem> queueItems = new List<QueueItem>();
                for(var i = 0; i < amountOfTimes; i++)
                    queueItems = queueItems.Append(new QueueItem(subReddit.Id, subReddit.Name, channelId, post, Guid.NewGuid(), sort));

                _queueManagerService.AddToQueue(queueItems, guildId);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                LazyAddToQueue.Release();
            }
        }

        public IEnumerable<QueueItem> GetByGuildId(ulong guildId)
        {
            return _queueManagerService.GetGuildQueueItems(guildId);
        }

        public void ClearChannelQueue(ulong channelId)
        {
            _queueManagerService.RemoveByChannelId(channelId);
        }

        public async Task RemoveSubredditFromQueue(ulong channelId, string subredditName)
        {
            var subreddit = await _subRedditFactory.GetSubRedditByName(subredditName);

            if (subreddit is null)
                return;

            _queueManagerService.RemoveSubredditFromQueue(channelId, subreddit.Id);
        }

        private async Task<SubReddit> GetSubReddit(string subRedditName)
        {
            var subReddit = await _subRedditFactory.GetSubRedditByName(subRedditName);
            return subReddit;
        }

        #endregion
    }
}
