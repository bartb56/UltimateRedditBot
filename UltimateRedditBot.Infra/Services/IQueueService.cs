using System.Collections.Generic;
using System.Threading.Tasks;
using UltimateRedditBot.Domain.Queue;

namespace UltimateRedditBot.Infra.Services
{
    public interface IQueueService
    {
        Task ProcessQueue(IAsyncEnumerable<QueueItem> queueItems);
    }
}
