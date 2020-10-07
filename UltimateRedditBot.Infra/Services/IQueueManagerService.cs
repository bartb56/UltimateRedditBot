using System.Collections.Generic;
using UltimateRedditBot.Domain.Queue;

namespace UltimateRedditBot.Infra.Services
{
    public interface IQueueManagerService
    {
        void AddToQueue(IEnumerable<QueueItem> queueItems, ulong guildId);

        IEnumerable<QueueItem> GetGuildQueueItems(ulong guildId);

        void RemoveByChannelId(ulong channelId);

        void RemoveSubredditFromQueue(ulong channelId, int subredditId);
    }
}
