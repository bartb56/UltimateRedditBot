using EasyCaching.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UltimateRedditBot.Core.Constants;
using UltimateRedditBot.Domain.Queue;
using UltimateRedditBot.Infra.Services;

namespace UltimateRedditBot.Core.Services
{
    public class RedisCacheManager : IRedisCacheManager
    {
        #region Fields

        private readonly IEasyCachingProvider _easyCachingProvider;
        private static readonly SemaphoreSlim _queueAccessor = new SemaphoreSlim(1, 1);
        private readonly ILogger<RedisCacheManager> _logger;

        #endregion

        #region Constructor

        public RedisCacheManager(IEasyCachingProviderFactory easyCachingProviderFactory, ILogger<RedisCacheManager> logger)
        {
            _logger = logger;
            _easyCachingProvider = easyCachingProviderFactory.GetCachingProvider(CachingConstants.RedisName);
        }

        #endregion

        #region Methods

        public async Task<IEnumerable<QueueItem>> GetQueueItems(int? max = null)
        {
            await _queueAccessor.WaitAsync();
            try
            {
                var cachedResult = await _easyCachingProvider.GetAsync<IEnumerable<QueueItem>>(CachingConstants.QueueKey);

                if (cachedResult is null || cachedResult.Value is null)
                    return null;

                if(max.HasValue)
                    _logger.Log(LogLevel.Information, $"There are currently: { cachedResult.Value.Count() } in the queue");

                return cachedResult.Value;
            }
            finally
            {
                _queueAccessor.Release();
            }
        }

        public async Task InsertRangeQueueItems(IEnumerable<QueueItem> queueItems)
        {
            if (queueItems is null)
                return;

            var cachedQueueItems = await GetQueueItems();
            cachedQueueItems ??= new List<QueueItem>();

            await _queueAccessor.WaitAsync();
            try
            {
                var newQueue = queueItems.Concat(cachedQueueItems);
                await _easyCachingProvider.SetAsync(CachingConstants.QueueKey, newQueue.ToList(), TimeSpan.FromDays(1));
            }
            finally
            {
                _queueAccessor.Release();
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
            newQueue.RemoveAll(queueItem => queueItemsIds.Contains(queueItem.Id));

            await _queueAccessor.WaitAsync();
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
                _queueAccessor.Release();
            }
        }

        public async Task RemoveByGuildId(ulong guildId)
        {

            var cachedQueueItems = await GetQueueItems();
            if (cachedQueueItems is null)
                return;

            await _queueAccessor.WaitAsync();
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
                _queueAccessor.Release();
            }
        }

        public async Task<bool> Exists()
        {
            await _queueAccessor.WaitAsync();
            try
            {
                var cachedResult = await _easyCachingProvider.ExistsAsync(CachingConstants.QueueKey);
                return cachedResult;

            }
            finally
            {
                _queueAccessor.Release();
            }
        }

        #endregion


    }
}
