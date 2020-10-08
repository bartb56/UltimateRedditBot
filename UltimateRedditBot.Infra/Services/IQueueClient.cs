using System.Collections.Generic;
using UltimateRedditBot.Domain.Queue;

namespace UltimateRedditBot.Infra.Services
{
    public interface IQueueClient
    {
        ulong GuildId { get; }

        IEnumerable<QueueItem> QueueItems { get; set; }

        void RemoveByChannelId(ulong channelId);

        void RemoveBySubredditId(ulong channelId, int subredditId);
    }
}
