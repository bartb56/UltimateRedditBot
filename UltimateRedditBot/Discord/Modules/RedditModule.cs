using System.Collections.Generic;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using UltimateRedditBot.App.Constants;
using UltimateRedditBot.App.Factories.GuildSettingsFactory;
using UltimateRedditBot.App.Factories.QueueFactory;
using UltimateRedditBot.App.Factories.SubRedditFactory;
using UltimateRedditBot.App.Factories.SubRedditHistoryFactory;
using UltimateRedditBot.Domain.Constants;
using UltimateRedditBot.Domain.Queue;

namespace UltimateRedditBot.Discord.Modules
{
    /// <summary>
    /// The command handler for all the reddit related commands
    /// </summary>
    public class RedditModule : UltimateCommandModule
    {
        #region Fields

        private readonly IQueueFactory _queueFactory;
        private readonly IGuildSettingsFactory _guildSettingsFactory;
        private readonly ISubRedditHistoryFactory _subRedditHistoryFactory;
        private readonly ISubRedditFactory _subRedditFactory;

        #endregion

        #region Constructor

        public RedditModule(IQueueFactory queueFactory,
            IGuildSettingsFactory guildSettingsFactory, ISubRedditHistoryFactory subRedditHistoryFactory, ISubRedditFactory subRedditFactory)
        {
            _queueFactory = queueFactory;
            _guildSettingsFactory = guildSettingsFactory;
            _subRedditHistoryFactory = subRedditHistoryFactory;
            _subRedditFactory = subRedditFactory;
        }

        #endregion

        #region Commands

        /// <summary>
        /// Used to get a request a subreddit post.
        /// </summary>
        /// <param name="subreddit">requested subreddit</param>
        /// <returns></returns>
        [Command("Reddit"), Alias("R")]
        public async Task Reddit(string subreddit)
        {
            await Reddit(subreddit, 1);
        }

        /// <summary>
        /// Used to request a subreddit post multiple times at once.
        /// </summary>
        /// <param name="subreddit">requested subreddit</param>
        /// <param name="amountOfRequests">requested amount of posts</param>
        /// <returns></returns>
        [Command("Reddit"), Alias("R")]
        public async Task Reddit(string subreddit, int amountOfRequests)
        {
            var guild = ((SocketGuildChannel)Context.Channel).Guild;

            //If the amount of requests is more than one ensure that it doesn't exceed the configured max requests.
            if (amountOfRequests > 1)
            {
                var maxBulkSetting = await _guildSettingsFactory.GetGuildSettingByKey<int>(guild.Id, DefaultSettingKeys.Bulk);

                var max = maxBulkSetting > 0 ? maxBulkSetting : 20;

                if (amountOfRequests > max)
                {
                    await ReplyAsync($"You can't place more than {max} requests at once.");
                    return;
                }
            }

            await Context.Channel.TriggerTypingAsync();
            await _queueFactory.AddToQueue(guild.Id, subreddit, Domain.Models.PostType.Image, ((SocketGuildChannel)Context.Channel).Id, amountOfRequests);
        }

        /// <summary>
        /// Used to get the current queue.
        /// </summary>
        /// <returns></returns>
        [Command("Queue"), Alias("R-Q")]
        public async Task Queue()
        {
            var guildId = ((SocketGuildChannel)Context.Channel).Guild.Id;

            var queueItems = await _queueFactory.GetByGuildId(guildId);

            var embedBuilder = new EmbedBuilder()
            {
                Color = EmbedConstants.EmbedColor,
                Title = $"Queue items in {((SocketGuildChannel)Context.Channel).Guild.Name}",
                Fields = await PrepareEmbedFieldBuilders(queueItems)
            };

            await ReplyAsync("", false, embedBuilder.Build());
        }

        private async Task<List<EmbedFieldBuilder>> PrepareEmbedFieldBuilders(IEnumerable<QueueItem> queueItems)
        {
            var embedBuilder = new List<EmbedFieldBuilder>();

            foreach (var queueItem in queueItems.GroupBy(queueItem => queueItem.SubRedditId)
                .Select(group => new
                {
                    Subreddit = group.Key,
                    Count = group.Count()
                })
                .OrderBy(x => x.Subreddit))
            {
                var subreddit = await _subRedditFactory.GetById(queueItem.Subreddit);
                embedBuilder.Add(new EmbedFieldBuilder()
                {
                    Name = subreddit.Name,
                    Value = $"{queueItem.Count}, {((queueItem.Count > 1) ? "times" : "time")} in the queue"
                });

            }

            return embedBuilder;
        }

        /// <summary>
        /// Used to clear the current guilds queue.
        /// </summary>
        /// <returns></returns>
        [Command("Queue-Clear"), Alias("R-Q-C")]
        public async Task ClearQueue()
        {
            var guildId = ((SocketGuildChannel)Context.Channel).Guild.Id;

            await _queueFactory.ClearGuildQueue(guildId);
            await ReplyAsync("Cleared the queue");
        }

        /// <summary>
        /// Used to clear the saved subreddits history
        /// </summary>
        /// <param name="subReddit"></param>
        /// <returns></returns>
        [Command("r-reset"), Alias("R-r")]
        public async Task ClearHistory(string subReddit)
        {
            var guildId = ((SocketGuildChannel)Context.Channel).Guild.Id;

            var responseMessage = await _subRedditHistoryFactory.UnSubscribe(guildId, subReddit);
            await ReplyAsync(responseMessage);
        }

        #endregion
    }
}
