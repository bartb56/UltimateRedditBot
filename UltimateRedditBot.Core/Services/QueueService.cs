using Discord;
using Discord.WebSocket;
using EasyCaching.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UltimateRedditBot.Core.Extensions;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Domain.Queue;
using UltimateRedditBot.Infra.Services;
using UltimateRedditBot.Infra.Uow;

namespace UltimateRedditBot.Core.Services
{
    public class QueueService : IQueueService
    {
        #region Fields


        private readonly IRedditApiService _redditApiService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DiscordSocketClient _discordClient;
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private static SemaphoreSlim queueAccessor = new SemaphoreSlim(1, 1);
        private readonly IRedisCacheManager _redisCacheManager;

        #endregion

        #region Constructor

        public QueueService(IRedditApiService redditApiService, IUnitOfWork unitOfWork, DiscordSocketClient discordClient, IRedisCacheManager redisCacheManager)
        {
            _redditApiService = redditApiService;
            _unitOfWork = unitOfWork;
            _discordClient = discordClient;
            _redisCacheManager = redisCacheManager;
            Initialize();
        }

        #endregion

        #region Methods

        public async Task AddToQueue(QueueItem queueItem)
        {
        }

        public async Task AddToQueueRange(IEnumerable<QueueItem> queueItems)
        {
            int index = 0;
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


        public async Task<IEnumerable<QueueItem>> GetQueueByGuild(Guild guild)
        {
            //var queueItemsCache = await _easyCachingProvider.GetAsync<List<QueueItem>>("queue");

            //if (!queueItemsCache.HasValue)
            return null;

            //return queueItemsCache.Value.Where(queueItem => queueItem.GuildId == guild.Id);
        }

        private async Task Initialize()
        {
            await Task.Delay(2000);
            await ProccessQueue();
        }

        private async Task ProccessQueue()
        {
            var queueItemsCache = await _redisCacheManager.GetQueueItems();
            var queueItemsToBeProcessed = queueItemsCache?.DistinctBy(x => x.SubRedditId).ToList();

            if (queueItemsCache.Any())
            {
                Console.WriteLine($"There are currently: { queueItemsCache.Count() } in the queue");
                await semaphoreSlim.WaitAsync();
                try
                {
                    var postDtos = new List<Task<PostDto>>();

                    foreach (var queueItem in queueItemsToBeProcessed)
                    {
                        var subreddit = await _unitOfWork.SubRedditRepository.GetById(queueItem.SubRedditId);
                        var history = subreddit.SubRedditHistories.FirstOrDefault(x => x.GuildId == queueItem.GuildId);
                        if (history != null)
                        {
                            var lastPost = await _unitOfWork.PostRepository.GetById(history.LastPostId);
                            postDtos.Add(_redditApiService.GetPost(queueItem, lastPost.PostId, subreddit.Name));
                        }
                        else
                            postDtos.Add(_redditApiService.GetPost(queueItem, "", subreddit.Name));
                    }

                    var posts = await Task.WhenAll(postDtos.ToArray());
                    posts = posts.Where(post => post != null).ToArray();

                    if(posts is null)
                    {
                        semaphoreSlim.Release();
                        //Wait a couple of ms before going again.
                        await Task.Delay(500);
                        await ProccessQueue();
                        return;
                    }

                    foreach (var queueItem in queueItemsToBeProcessed)
                    {
                        //Get the post for this queueItem
                        var post = posts.FirstOrDefault(post => post.QueueItemId == queueItem.Id && post.Post?.SubRedditId == queueItem.SubRedditId)?.Post;
                        if (post is null)
                            continue;

                        //Send the post back to the client.
                        var replyMessageTask = ReplyMessage(queueItem.ChannelId, post.Url.ToString());

                        //Save the post details
                        await SaveOrGetPost(post);
                            
                        //Update the guild's history.
                        await UpdateGuildHistory(post, queueItem.GuildId, queueItem.SubRedditId);

                        //Ensure that the message has been send before continueing,
                        await replyMessageTask;
                    }
                    
                    //Commit the database changes.
                    _unitOfWork.Commit();

                    //To minimize the time that i have to lock the queue service ill get the queue items here again.
                    await _redisCacheManager.RemoveRangeQueueItems(queueItemsToBeProcessed.Select(x => x.Id));
                }
                catch (Exception e)
                {

                }
                finally
                {
                    semaphoreSlim.Release();
                }

                //Wait a couple of ms before going again.
                await Task.Delay(500);
                await ProccessQueue();
            }
            else
            {
                await Task.Delay(500);
                await ProccessQueue();
            }
        }

        private async Task ReplyMessage(ulong channelId, string postUrl)
        {
            var channel = _discordClient.GetChannel(channelId) as ITextChannel;
            await channel.SendMessageAsync(postUrl);
        }

        private async Task SaveOrGetPost(Post post)
        {
            if (await _unitOfWork.PostRepository.IsUniquePost(post))
            {
                await _unitOfWork.PostRepository.Insert(post);
                _unitOfWork.Commit();
            }
            else
            {
                post = await _unitOfWork.PostRepository.GetByPostId(post.PostId);
            }
        }

        private async Task UpdateGuildHistory(Post post, ulong guildId, int subRedditId)
        {
            var history = await _unitOfWork.SubRedditHistoryRepository.Queriable().FirstOrDefaultAsync(x => x.GuildId == guildId && x.SubRedditId == subRedditId);
            if (history == null)
            
                await _unitOfWork.SubRedditHistoryRepository.Insert(new SubRedditHistory(post.Id, guildId, subRedditId));
            else
            {
                history.UpdateLastPostId(post.Id);
                await _unitOfWork.SubRedditHistoryRepository.Update(history);
            }

            _unitOfWork.Commit();
        }

        private async Task Refactor()
        {
            //var queueItemsCache = await _easyCachingProvider.GetAsync<List<QueueItem>>("queue");
        }

        #endregion
    }
}
