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

            while (index * 500 < max)
            {
                var amountOfQueueItemsAdded = index * 500;
                var queueItemsToAdd = queueItems.Skip(amountOfQueueItemsAdded).Take(500);
                await _redisCacheManager.InsertRangeQueueItems(queueItemsToAdd);
                index++;

                await Task.Delay(500);
            }
        }

        public async Task ClearGuildQueue(int guildId)
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
            if (queueItemsCache != null && queueItemsCache.Any())
            {
                Console.WriteLine($"There are currently: { queueItemsCache.Count() } in the queue");

                await semaphoreSlim.WaitAsync();
                try
                {
                    var queueItemsToBeProcessed = queueItemsCache.DistinctBy(x => x.SubRedditId).ToList();
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

                    if (posts != null)
                    {
                        foreach (var queueItem in queueItemsToBeProcessed)
                        {
                            var postDto = posts.FirstOrDefault(post => post.QueueItemId == queueItem.Id && post.Post?.SubRedditId == queueItem.SubRedditId);

                            if (postDto == null || postDto.Post == null)
                                continue;

                            var post = postDto.Post;

                            if (await _unitOfWork.PostRepository.IsUniquePost(post))
                            {
                                await _unitOfWork.PostRepository.Insert(post);
                                _unitOfWork.Commit();
                            }
                            else
                            {
                                post = await _unitOfWork.PostRepository.GetByPostId(post.PostId);
                            }

                            var history = await _unitOfWork.SubRedditHistoryRepository.Queriable().FirstOrDefaultAsync(x => x.GuildId == queueItem.GuildId && x.SubRedditId == queueItem.SubRedditId);
                            if (history == null)
                            {
                                await _unitOfWork.SubRedditHistoryRepository.Insert(new SubRedditHistory(post.Id, queueItem.GuildId, queueItem.SubRedditId));
                                _unitOfWork.Commit();
                            }

                            else
                            {
                                history.UpdateLastPostId(post.Id);
                                await _unitOfWork.SubRedditHistoryRepository.Update(history);
                            }
                            var channel = _discordClient.GetChannel(queueItem.ChannelId) as ITextChannel;
                            await channel.SendMessageAsync(post.Url.ToString());
                        }
                    }
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


        private async Task Refactor()
        {
            //var queueItemsCache = await _easyCachingProvider.GetAsync<List<QueueItem>>("queue");
        }

        #endregion
    }
}
