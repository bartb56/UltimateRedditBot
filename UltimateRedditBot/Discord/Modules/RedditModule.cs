using Discord;
using Discord.Commands;
using Discord.WebSocket;
using EasyCaching.Core;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateRedditBot.App.Constants;
using UltimateRedditBot.App.Factories.GuildSettingsFactory;
using UltimateRedditBot.App.Factories.QueueFactory;
using UltimateRedditBot.Infra.Services;
using UltimateRedditBot.Infra.Uow;

namespace UltimateRedditBot.Discord.Modules
{
    public class RedditModule : UltimateCommandModule
    {
        #region Fields

        private readonly DiscordSocketClient _discord;
        private readonly IQueueFactory _queueFactory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGuildSettingsFactory _guildSettingsFactory;

        private readonly IEasyCachingProvider _easyCachingProvider;
        private readonly IEasyCachingProviderFactory _easyCachingProviderFactory;

        #endregion

        #region Constructor

        public RedditModule(
                    DiscordSocketClient discord, IQueueFactory queueFactory,
                    IUnitOfWork unitOfWork,
                    IGuildSettingsFactory guildSettingsFactory,

                    IEasyCachingProviderFactory easyCachingProviderFactory)
        {
            _discord = discord;
            _queueFactory = queueFactory;
            _unitOfWork = unitOfWork;
            _guildSettingsFactory = guildSettingsFactory;

            
            _easyCachingProviderFactory = easyCachingProviderFactory;
            _easyCachingProvider = easyCachingProviderFactory.GetCachingProvider("redis1");
        }

        #endregion


        #region Commands

        [Command("Reddit"), Alias("R"), ]
        public async Task Reddit(string subreddit)
        {
            await Reddit(subreddit, 1);
        }

        [Command("Reddit"), Alias("R")]
        public async Task Reddit(string subreddit, int amountOfTimes)
        {
            SocketGuild guild = ((SocketGuildChannel)Context.Channel).Guild;

            var settingsMax = await _guildSettingsFactory.GetGuildSettingByKey<int>(guild.Id, DefaultSettingKeys.Bulk);

            var max = (settingsMax > 0) ? settingsMax : 20;

            if (amountOfTimes > max)
            {
                await SendReplyMessage(string.Format("You can't place more than {0} requests at once.", max));
                return;
            }

            await Context.Channel.TriggerTypingAsync();

            await _queueFactory.AddToQueue(guild.Id, subreddit, Domain.Models.PostType.Image, ((SocketGuildChannel)Context.Channel).Id, amountOfTimes);
        }

        [Command("Queue"), Alias("R-Q")]
        public async Task Queue()
        {
            var guildId = ((SocketGuildChannel)Context.Channel).Guild.Id;

            var queueItems = await _queueFactory.GetByGuildId(guildId);
            if (!queueItems.ToList().Any())
            {
                await SendReplyMessage("Queue is empty");
                return;
            }

            var queueBuilder = new StringBuilder();
            foreach (var queueItem in queueItems)
            {
                queueBuilder.Append($"{queueItem.SubRedditId}, {queueItem.PostType}");
            }

            await SendReplyMessage(queueBuilder.ToString());
        }

        [Command("Queue-Clear"), Alias("R-Q-C")]
        public async Task ClearQueue()
        {
            var guildId = ((SocketGuildChannel)Context.Channel).Guild.Id;

            await _queueFactory.ClearGuildQueue(guildId);
            await SendReplyMessage("Cleared the queue");
        }

        [Command("r-c"), Alias("R-c")]
        public async Task ClearHistory(string subReddit)
        {
            var guildId = ((SocketGuildChannel)Context.Channel).Guild.Id;

            var guilds = await _unitOfWork.GuildRepository.GetAll();
            var xt = guilds.FirstOrDefault(x => x.Id == guildId);

            var subreddit = await _unitOfWork.SubRedditRepository.GetSubRedditByName(subReddit);
            var history = await _unitOfWork.SubRedditHistoryRepository.GetAll();

            await _unitOfWork.SubRedditHistoryRepository.Delete(history.FirstOrDefault(x => x.SubRedditId == subreddit.Id && x.GuildId == xt.Id));
            _unitOfWork.Commit();
            await SendReplyMessage("done");
        }

        [Command("r-invite")]
        public async Task SendInvite()
        {
            await SendReplyMessage("https://discord.com/api/oauth2/authorize?client_id=714492192319733811&permissions=2048&scope=bot");
            return;
        }

        private async Task<IUserMessage> SendReplyMessage(string message)
        {
            return await ReplyAsync(message);
        }

        #endregion
    }
}
