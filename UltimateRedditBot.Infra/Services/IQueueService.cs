using System.Collections.Generic;
using System.Threading.Tasks;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Domain.Queue;

namespace UltimateRedditBot.Infra.Services
{
    public interface IQueueService
    {
        Task AddToQueue(QueueItem queueItem);

        Task AddToQueueRange(IEnumerable<QueueItem> queueItem);

        Task<IEnumerable<QueueItem>> GetQueueByGuild(Guild guild);

        Task ClearGuildQueue(ulong guildId);
    }
}
