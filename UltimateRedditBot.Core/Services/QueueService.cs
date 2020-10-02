using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using UltimateRedditBot.Core.Extensions;
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
        private readonly IRedisCacheManager _redisCacheManager;
        private readonly IServiceProvider _serviceProvider;


        #endregion

        #region Constructor

        public QueueService(IServiceProvider serviceProvider, IRedditApiService redditApiService, DiscordSocketClient discordClient, IRedisCacheManager redisCacheManager)
        {
            _serviceProvider = serviceProvider;
            _redditApiService = redditApiService;
            _discordClient = discordClient;
            _redisCacheManager = redisCacheManager;
        }

        #endregion

        #region Methods

        public async Task AddToQueueRange(IEnumerable<QueueItem> queueItems)
        {
            queueItems = queueItems.ToList();
            var index = 0;
            var max = queueItems.Count();

            while (index * 1000000 < max)
            {
                var amountOfQueueItemsAdded = index * 1000000;
                var queueItemsToAdd = queueItems.Skip(amountOfQueueItemsAdded).Take(1000000);
                await _redisCacheManager.InsertRangeQueueItems(queueItemsToAdd);
                index++;

                await Task.Delay(500);
            }
        }

        public async Task ClearGuildQueue(ulong guildId)
        {
            await _redisCacheManager.RemoveByGuildId(guildId);
        }


        public async Task<IEnumerable<QueueItem>> GetQueueByGuild(ulong guildId)
        {
            var queueItems = await _redisCacheManager.GetQueueItems();
            queueItems = queueItems?.Where(x => x.GuildId == guildId);

            return queueItems;
        }

        public async Task ProcessQueue()
        {
            var hasQueueItems = await _redisCacheManager.Exists();
            if (!hasQueueItems)
                return;

            var queueItemsCache = await _redisCacheManager.GetQueueItems(0);

            var queueItems = queueItemsCache.DistinctBy(x => new { x.SubRedditId, x.GuildId });

            await ProccessQueue(queueItems.ToAsyncEnumerable());
        }

        private async Task ProccessQueue(IAsyncEnumerable<QueueItem> queueItems)
        {
            var postDtos = await GetPostDtos(queueItems);

            var posts = await Task.WhenAll(postDtos.ToArray());
            posts = posts.Where(post => post is not null).ToArray();

            if (!posts.Any())
            {
                return;
            }

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
                await UpdateGuildHistory(post, queueItem.GuildId, queueItem.SubRedditId);

                //Ensure that the message has been send before continueing,
                await replyMessageTask;
            }

            //To minimize the time that i have to lock the queue service ill get the queue items here again.
            await _redisCacheManager.RemoveRangeQueueItems(await queueItems.Select(x => x.Id).ToListAsync());
        }

        private async Task<IEnumerable<Task<PostDto>>> GetPostDtos(IAsyncEnumerable<QueueItem> queueItems)
        {
            var postDtos = new List<Task<PostDto>>();

            await foreach (var queueItem in queueItems)
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<Context>();
                var subreddit = await context.SubReddits.Include(x => x.SubRedditHistories)
                    .FirstOrDefaultAsync(x => x.Id == queueItem.SubRedditId);

                var history = subreddit.SubRedditHistories.FirstOrDefault(x => x.GuildId == queueItem.GuildId);
                if (history != null && !string.IsNullOrEmpty(history.LastPostId))
                {
                    var lastPost = await context.Posts.FindAsync(history.LastPostId);
                    postDtos.Add(_redditApiService.GetOldPost(subreddit.Name, lastPost.Id, queueItem.Sort,
                        queueItem.PostType, queueItem.Id));
                }
                else
                    postDtos.Add(_redditApiService.GetOldPost(subreddit.Name, "", queueItem.Sort,
                        queueItem.PostType, queueItem.Id));
            }

            return postDtos;
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

        #endregion
    }
}
