using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UltimateRedditBot.Domain.Queue;

namespace UltimateRedditBot.Infra.Services
{
    public interface IRedisCacheManager
    {

        Task<IEnumerable<QueueItem>> GetQueueItems();

        Task InsertRangeQueueItems(IEnumerable<QueueItem> queueItems);

        Task RemoveRangeQueueItems(IEnumerable<Guid> queueItemsIds);

        Task RemoveByGuildId(int guildId);
    }
}
