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

        public async Task<IEnumerable<QueueItem>> GetQueueItems()
        {
            await queueAccessor.WaitAsync();
            try
            {
                var cachedResult = await _easyCachingProvider.GetAsync<IEnumerable<QueueItem>>(CachingConstants.QueueKey);
                return cachedResult.Value;

            }
            finally
            {
                queueAccessor.Release();
            }
        }

        public async Task InsertRangeQueueItems(IEnumerable<QueueItem> queueItems)
        {
            if (queueItems == null)
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
            if (queueItemsIds == null)
                return;

            queueItemsIds = queueItemsIds.ToList();

            var cachedQueueItems = await GetQueueItems();
            cachedQueueItems ??= new List<QueueItem>();

            await queueAccessor.WaitAsync();
            try
            {
                var newQue = cachedQueueItems.ToList();
                newQue.Remove(newQue.FirstOrDefault(x => queueItemsIds.Any(y => x.Id == y)));

                await _easyCachingProvider.SetAsync(CachingConstants.QueueKey, newQue, TimeSpan.FromDays(1));
            }
            finally
            {
                queueAccessor.Release();
            }
        }

        public async Task RemoveByGuildId(ulong guildId)
        {

            var cachedQueueItems = await GetQueueItems();
            if (cachedQueueItems == null)
                return;

            await queueAccessor.WaitAsync();
            try
            {
                var newQueue = cachedQueueItems.Where(x => x.GuildId != guildId);
                await _easyCachingProvider.SetAsync(CachingConstants.QueueKey, newQueue.ToList(), TimeSpan.FromDays(1));
            }
            finally
            {
                queueAccessor.Release();
            }
        }

        #endregion


    }
}
