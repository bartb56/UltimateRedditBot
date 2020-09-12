using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UltimateRedditBot.App.Factories.SubRedditFactory;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Domain.Queue;
using UltimateRedditBot.Infra.Services;
using UltimateRedditBot.Infra.Uow;

namespace UltimateRedditBot.App.Factories.QueueFactory
{
    public class QueueFactory : IQueueFactory
    {
        #region Fields

        private readonly IQueueService _queueService;
        private readonly ISubRedditFactory _subRedditFactory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DiscordSocketClient _discord;

        static SemaphoreSlim lazyAddToQueue = new SemaphoreSlim(1, 1);

        #endregion

        #region Constructor

        public QueueFactory(IQueueService queueService,
            ISubRedditFactory subRedditFactory,
            IUnitOfWork unitOfWork,
            DiscordSocketClient discord)
        {
            _queueService = queueService;
            _subRedditFactory = subRedditFactory;
            _unitOfWork = unitOfWork;
            _discord = discord;
        }

        #endregion

        #region Methods

        public async Task AddToQueue(ulong guildId, string subRedditName, PostType post, ulong channelId, int amountOfTimes)
        {
            await lazyAddToQueue.WaitAsync();
            try
            {
                var txtChannel = _discord.GetChannel(channelId) as ITextChannel;
                var subReddit = await GetSubReddit(subRedditName);
                if (subReddit is null)
                {
                    await txtChannel.SendMessageAsync(text: "The subreddit could not be found.");
                    lazyAddToQueue.Release();
                    return;
                }

                if(subReddit.IsNsfw && !txtChannel.IsNsfw)
                {
                    await txtChannel.SendMessageAsync(text: "This subreddit can only be used in nsfw channels.");
                    lazyAddToQueue.Release();
                    return;
                }

                IEnumerable<QueueItem> queueItems = new List<QueueItem>();
                for(var i = 0; i < amountOfTimes; i++)
                    queueItems = queueItems.Append(new QueueItem(guildId, subReddit.Id, channelId, post, Guid.NewGuid()));
                
                await _queueService.AddToQueueRange(queueItems);
            }
            catch(Exception e)
            {

            }
            finally
            {
                lazyAddToQueue.Release();
            }
            
        }

        public async Task<IEnumerable<QueueItem>> GetByGuildId(ulong guildId)
        {
            var guild = await _unitOfWork.GuildRepository.GetById(guildId);
            return await _queueService.GetQueueByGuild(guild);
        }

        public async Task ClearGuildQueue(ulong guildId)
        {
            var guild = await _unitOfWork.GuildRepository.GetById(guildId);
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
