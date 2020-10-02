using System.Collections.Generic;
using System.Threading.Tasks;
using UltimateRedditBot.Domain.Queue;

namespace UltimateRedditBot.Infra.Services
{
    public interface IQueueService
    {
        Task AddToQueueRange(IEnumerable<QueueItem> queueItem);

        Task<IEnumerable<QueueItem>> GetQueueByGuild(ulong guildId);

        Task ClearGuildQueue(ulong guildId);

        Task ProcessQueue();
    }
}
