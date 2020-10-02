using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using UltimateRedditBot.App.Factories.Subscriptions;
using UltimateRedditBot.Domain.Constants;
using UltimateRedditBot.Domain.Enums;
using UltimateRedditBot.Domain.Models;

namespace UltimateRedditBot.Discord.Modules
{
    /// <summary>
    /// The command handler for all the subscription related commands.
    /// </summary>
    public class SubscriptionModule : UltimateCommandModule
    {
        #region Fields

        private readonly ISubscriptionsFactory _subscriptionsFactory;

        #endregion

        #region Constructor

        public SubscriptionModule(ISubscriptionsFactory subscriptionsFactory)
        {
            _subscriptionsFactory = subscriptionsFactory;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the current subscribed subscriptions in a channel.
        /// </summary>
        /// <returns></returns>
        [Command("R-Subscriptions"), Alias("r-subs")]
        public async Task Subscriptions()
        {
            if (!((SocketGuildChannel) Context.Channel is ITextChannel channel))
                return;

            var subscriptions = await _subscriptionsFactory.Subscriptions(channel.Id);
            var embedFields = new List<EmbedFieldBuilder>();

            var subs = subscriptions.ToList();
            if (!subs.Any())
            {
                await ReplyAsync("There are no subscriptions in this channel");
                return;
            }

            var nsfwSubreddits = subs.Where(x => x.SubReddit.IsNsfw).ToList();
            if(nsfwSubreddits.Any())
                embedFields.Add(EmbedFieldBuilder(nsfwSubreddits, "Subreddits: "));

            var normalSubReddit = subs.Where(x => !x.SubReddit.IsNsfw).ToList();
            if(normalSubReddit.Any())
                embedFields.Add(EmbedFieldBuilder(normalSubReddit, "Subreddits: "));

            var embedBuilder = new EmbedBuilder
            {
                Color = EmbedConstants.EmbedColor,
                Title = $"Subscriptions in: { channel.Name }",
                Fields = embedFields
            };

            await ReplyAsync("", false, embedBuilder.Build());
        }

        /// <summary>
        /// Created an embed field builder for a list of subscriptions
        /// </summary>
        /// <param name="subscriptions">Subscribed subreddits</param>
        /// <param name="title">The field name / title</param>
        /// <returns></returns>
        private static EmbedFieldBuilder EmbedFieldBuilder(IEnumerable<Subscription> subscriptions, string title)
        {
            var builder = new EmbedFieldBuilder
            {
                Name = title,
                Value = AddSubscriptionValues(subscriptions.Select(x => x.SubReddit.Name))
            };

            return builder;
        }

        /// <summary>
        /// Parses all the subreddit names to a string.
        /// </summary>
        /// <param name="subredditNames"></param>
        /// <returns></returns>
        private static string AddSubscriptionValues(IEnumerable<string> subredditNames)
        {
            var valueBuilder = new StringBuilder();
            foreach (var subredditName in subredditNames)
                valueBuilder.Append($"{subredditName} \n");

            return valueBuilder.ToString();
        }

        /// <summary>
        /// Subscribe to a subscription.
        /// </summary>
        /// <param name="subReddit"></param>
        /// <returns></returns>
        [Command("R-subscribe"), Alias("r-sub")]
        public async Task Subscribe(string subReddit)
        {
            await Subscribe(subReddit, "New");
        }


        /// <summary>
        /// Subscribe to a subscription
        /// </summary>
        /// <param name="subReddit">requested subscription</param>
        /// <param name="sortUnParsed">sort by either hot or new</param>
        /// <returns></returns>
        [Command("R-subscribe"), Alias("r-sub")]
        public async Task Subscribe(string subReddit, string sortUnParsed)
        {
            //Ensure that the user entered a subreddit.
            if (string.IsNullOrWhiteSpace(subReddit))
            {
                await ReplyAsync("Please enter a subreddit name");
                return;
            }

            //Ensure that the sort param is parsed correctly.
            if (Enum.TryParse<Sort>(sortUnParsed, true, out var sort) && !string.IsNullOrEmpty(subReddit))
            {
                if (!((SocketGuildChannel)Context.Channel is ITextChannel channel))
                    return;

                await _subscriptionsFactory.Subscribe(channel.Id, subReddit, sort);
                return;
            }

            await ReplyAsync("Please use either New or Hot for sorting");
        }

        /// <summary>
        /// Used to unsubscribe from a subreddit
        /// </summary>
        /// <param name="subReddit">the requested subreddit.</param>
        /// <returns></returns>
        [Command("R-unsubscribe"), Alias("r-unsub")]
        public async Task UnSubscribe(string subReddit)
        {
            //Validation
            if (string.IsNullOrEmpty(subReddit))
            {
                await ReplyAsync("Please enter a subreddit name");
                return;
            }

            if (!((SocketGuildChannel)Context.Channel is ITextChannel channel))
                return;

            await _subscriptionsFactory.Unsubscribe(channel.Id, subReddit);
        }

        #endregion

    }
}
