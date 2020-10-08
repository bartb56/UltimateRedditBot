using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using UltimateRedditBot.Domain.Constants;

namespace UltimateRedditBot.Discord.Modules
{
    public class HelperModule : UltimateCommandModule
    {
        /// <summary>
        /// Gets the url used to add a bot to a new server.
        /// </summary>
        /// <returns></returns>
        [Command("r-invite"), Alias("r-inv")]
        public async Task GetInviteUrl()
        {
            await ReplyAsync(
                "https://discord.com/api/oauth2/authorize?client_id=714492192319733811&permissions=2048&scope=bot");
        }

        /// <summary>
        /// Shows a message with the available commands
        /// </summary>
        /// <returns></returns>
        [Command("r-help")]
        public async Task Help()
        {
            var fields = new List<EmbedFieldBuilder> { RedditPosts, Queue, Subscriptions, Settings, Extra };

            var footer = new EmbedFooterBuilder
            {
                Text = "Made with ❤️ by Bartb56#0734"
            };
 
            var helpEmbedBuilder = new EmbedBuilder
            {
                Color = EmbedConstants.EmbedColor,
                Title = "Help",
                Fields = fields,
                Footer = footer
            };

            await ReplyAsync("", false, helpEmbedBuilder.Build());
        }

        #region EmbedFieldBuilder

        private static EmbedFieldBuilder RedditPosts =>
            new EmbedFieldBuilder
            {
                Name = "Reddit posts",
                Value = "`$r (subreddit name)                   :` Get a reddit post \n `$r (subreddit name) (amount of posts) :` Get multiple reddit posts \n `$r-c (subreddit name)                 :` Clear a subreddits history"
            };

        private static EmbedFieldBuilder Queue =>
            new EmbedFieldBuilder
            {
                Name = "Queue",
                Value = "`$r-q                                  :` Queued items \n `$r-q-c                                :` Clear the queue"
            };

        private static EmbedFieldBuilder Subscriptions =>
            new EmbedFieldBuilder
            {
                Name = "Subscriptions",
                Value = "`$r-sub (subreddit name)               :`Subscribe to a subreddit \n `$r-subs                               :`View subscriptions \n `$r-unsub (subreddit name)             :`Unsubscribe \n"
            };

        private static EmbedFieldBuilder Settings =>
            new EmbedFieldBuilder
            {
                Name = "Settings",
                Value = "`$r-bulk                               :`Get the settings value \n `$r-bulk (new value)                   :`Update the setting \n \n `$r-sort                               :`Get the settings value \n `$r-sort (new value)                   :`Update the setting "
            };

        private static EmbedFieldBuilder Extra =>
            new EmbedFieldBuilder
            {
                Name = "Extra",
                Value = "`$r-inv                                :` Get the bot invite url"
            };

        #endregion
    }
}
