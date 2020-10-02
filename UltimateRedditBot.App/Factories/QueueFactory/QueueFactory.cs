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

        private readonly IQueueService _queueService;
        private readonly ISubRedditFactory _subRedditFactory;
        private readonly DiscordSocketClient _discord;
        private readonly IGuildAppService _guildAppService;
        private readonly IGuildSettingsAppService _guildSettingsAppService;

        private static readonly SemaphoreSlim LazyAddToQueue = new SemaphoreSlim(1, 1);

        #endregion

        #region Constructor

        public QueueFactory(IQueueService queueService,
            ISubRedditFactory subRedditFactory,
            DiscordSocketClient discord, IGuildAppService guildAppService, IGuildSettingsAppService guildSettingsAppService)
        {
            _queueService = queueService;
            _subRedditFactory = subRedditFactory;
            _discord = discord;
            _guildAppService = guildAppService;
            _guildSettingsAppService = guildSettingsAppService;
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
                    queueItems = queueItems.Append(new QueueItem(guildId, subReddit.Id, subReddit.Name, channelId, post, Guid.NewGuid(), sort));

                await _queueService.AddToQueueRange(queueItems);
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

        public async Task<IEnumerable<QueueItem>> GetByGuildId(ulong guildId)
        {
            return await _queueService.GetQueueByGuild(guildId);
        }

        public async Task ClearGuildQueue(ulong guildId)
        {
            var guild = await _guildAppService.GetById(guildId);
            await _queueService.ClearGuildQueue(guild.Id);
        }

        private async Task<SubReddit> GetSubReddit(string subRedditName)
        {
            var subReddit = await _subRedditFactory.GetSubRedditByName(subRedditName);
            return subReddit;
        }

        #endregion
    }
}
