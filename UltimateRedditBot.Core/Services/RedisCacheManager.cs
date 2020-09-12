using EasyCaching.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UltimateRedditBot.Core.Constants;
using UltimateRedditBot.Domain.Queue;
using UltimateRedditBot.Infra.Services;

namespace UltimateRedditBot.Core.Services
{
    public class RedisCacheManager : IRedisCacheManager
    {
        #region Fields

        private readonly IEasyCachingProvider _easyCachingProvider;
        private static SemaphoreSlim queueAccessor = new SemaphoreSlim(1, 1);

        #endregion

        #region Constructor

        public RedisCacheManager(IEasyCachingProviderFactory easyCachingProviderFactory)
        {
            _easyCachingProvider = easyCachingProviderFactory.GetCachingProvider(CachingConstants.RedisName);
        }

        #endregion

        #region Methods

        public async Task<IEnumerable<QueueItem>> GetQueueItems(int? max = null)
        {
            await queueAccessor.WaitAsync();
            try
            {
                var cachedResult = await _easyCachingProvider.GetAsync<IEnumerable<QueueItem>>(CachingConstants.QueueKey);

                if(cachedResult is not null && cachedResult.Value is not null && max.HasValue)
                    Console.WriteLine($"There are currently: { cachedResult.Value.Count() } in the queue");
                
                if(max.HasValue)
                    return cachedResult.Value;

                return cachedResult.Value;
            }
            finally
            {
                queueAccessor.Release();
            }
        }

        public async Task InsertRangeQueueItems(IEnumerable<QueueItem> queueItems)
        {
            if (queueItems is null)
                return;

            var cachedQueueItems = await GetQueueItems();
            cachedQueueItems ??= new List<QueueItem>();

            await queueAccessor.WaitAsync();
            try
            {
                var newQueue = queueItems.Concat(cachedQueueItems);
                await _easyCachingProvider.SetAsync(CachingConstants.QueueKey, newQueue.ToList(), TimeSpan.FromDays(1));
            }
            finally
            {
                queueAccessor.Release();
            }
        }

        public async Task RemoveRangeQueueItems(IEnumerable<Guid> queueItemsIds)
        {
            if (queueItemsIds is null)
                return;

            var cachedQueueItems = await GetQueueItems();
            if (cachedQueueItems is null)
                return;

            queueItemsIds = queueItemsIds.ToList();

            var newQueue = cachedQueueItems.ToList();
            newQueue.RemoveAll(queueItem => queueItemsIds.Contains(queueItem.Id));;

            await queueAccessor.WaitAsync();
            try
            {
                if (!newQueue.Any())
                    await _easyCachingProvider.RemoveAsync(CachingConstants.QueueKey);
                else
                    await _easyCachingProvider.SetAsync(CachingConstants.QueueKey, newQueue, TimeSpan.FromDays(1));
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                queueAccessor.Release();
            }
        }

        public async Task RemoveByGuildId(ulong guildId)
        {

            var cachedQueueItems = await GetQueueItems();
            if (cachedQueueItems is null)
                return;

            await queueAccessor.WaitAsync();
            try
            {
                var newQueue = cachedQueueItems.Where(x => x.GuildId != guildId).ToList();

                if (!newQueue.Any())
                    await _easyCachingProvider.RemoveAsync(CachingConstants.QueueKey);
                else
                    await _easyCachingProvider.SetAsync(CachingConstants.QueueKey, newQueue, TimeSpan.FromDays(1));
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                queueAccessor.Release();
            }
        }

        public async Task<bool> Exists()
        {
            await queueAccessor.WaitAsync();
            try
            {
                var cachedResult = await _easyCachingProvider.ExistsAsync(CachingConstants.QueueKey);
                return cachedResult;

            }
            finally
            {
                queueAccessor.Release();
            }
        }

        #endregion


    }
}
