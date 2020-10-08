using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using UltimateRedditBot.Database;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Domain.Queue;
using UltimateRedditBot.Infra.Services;

namespace UltimateRedditBot.Core.Services
{
    public class QueueService : IQueueService
    {
        #region Fields


        private readonly IRedditApiService _redditApiService;
        private readonly DiscordSocketClient _discordClient;
        private readonly IServiceProvider _serviceProvider;


        #endregion

        #region Constructor

        public QueueService(IServiceProvider serviceProvider, IRedditApiService redditApiService, DiscordSocketClient discordClient)
        {
            _serviceProvider = serviceProvider;
            _redditApiService = redditApiService;
            _discordClient = discordClient;
        }

        #endregion

        #region Methods

        public async Task ProcessQueue(IAsyncEnumerable<QueueItem> queueItems)
        {
            var postDtoTasks = await GetPostDtoTasks(queueItems);

            var posts = await Task.WhenAll(postDtoTasks.ToArray());
            posts = posts.Where(post => post is not null).ToArray();

            if (!posts.Any())
                return;

            await foreach (var queueItem in queueItems)
            {
                //Get the post for this queueItem
                var post = posts.FirstOrDefault(x => x.QueueItemId == queueItem.Id)?.Post;
                if (post is null)
                    continue;

                post.SubRedditId = queueItem.SubRedditId;

                //Send the post back to the client.
                var replyMessageTask = ReplyMessage(queueItem.ChannelId, queueItem.SubredditName, post);

                //Save the post details
                post = await SaveOrGetPost(post);

                //Update the guild's history.
                await UpdateGuildHistory(post, GetGuildIdFromChannelId(queueItem.ChannelId), queueItem.SubRedditId);

                //Ensure that the message has been send before returning,
                await replyMessageTask;
            }
        }

        private async Task<IEnumerable<Task<PostDto>>> GetPostDtoTasks(IAsyncEnumerable<QueueItem> queueItems)
        {
            var postDtoTasks = new List<Task<PostDto>>();

            await foreach (var queueItem in queueItems)
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<Context>();
                var subreddit = await context.SubReddits.Include(x => x.SubRedditHistories)
                    .FirstOrDefaultAsync(x => x.Id == queueItem.SubRedditId);

                var history = subreddit.SubRedditHistories.FirstOrDefault(x => x.GuildId == GetGuildIdFromChannelId(queueItem.ChannelId));
                if (history != null && !string.IsNullOrEmpty(history.LastPostId))
                    postDtoTasks.Add(_redditApiService.GetOldPost(subreddit.Name, history.LastPostId, queueItem.Sort,
                        queueItem.PostType, queueItem.Id));

                else
                    postDtoTasks.Add(_redditApiService.GetOldPost(subreddit.Name, "", queueItem.Sort,
                        queueItem.PostType, queueItem.Id));
            }

            return postDtoTasks;
        }

        private async Task ReplyMessage(ulong channelId, string subreddit, Post post)
        {
            if (_discordClient.GetChannel(channelId) is ITextChannel channel)
                await channel.SendMessageAsync("", false, post.Embed(subreddit));
        }

        private async Task<Post> SaveOrGetPost(Post post)
        {
            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<Context>();
            if (await context.Posts.FindAsync(post.Id) is null)
            {
                await context.Posts.AddAsync(post);
                await context.SaveChangesAsync();
            }
            else
                post = await context.Posts.FindAsync(post.Id);

            return post;
        }

        private async Task UpdateGuildHistory(Post post, ulong guildId, int subRedditId)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Context>();
            var history =
                context.SubRedditHistories.FirstOrDefault(x =>
                    x.GuildId == guildId && x.SubRedditId == subRedditId);

            if (history is null)
            {
                await context.SubRedditHistories.AddAsync(new SubRedditHistory(post.Id, guildId, subRedditId));
                await context.SaveChangesAsync();
            }
            else
            {
                history.UpdateLastPostId(post.Id);
                context.SubRedditHistories.Update(history);
                await context.SaveChangesAsync();
            }
        }

        private ulong GetGuildIdFromChannelId(ulong channelId)
        {
            var channel = _discordClient.GetChannel(channelId) as ITextChannel;
            Debug.Assert(channel != null, nameof(channel) + " != null");
            return channel.GuildId;
        }

        #endregion
    }
}
