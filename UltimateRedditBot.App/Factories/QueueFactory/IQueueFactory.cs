﻿using System.Collections.Generic;
using System.Threading.Tasks;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Domain.Queue;

namespace UltimateRedditBot.App.Factories.QueueFactory
{
    public interface IQueueFactory
    {
        Task AddToQueue(ulong guildId, string subRedditName, PostType post, ulong channelId, int amountOfTimes);

        IEnumerable<QueueItem> GetByGuildId(ulong guildId);

        void ClearChannelQueue(ulong channelId);

        Task RemoveSubredditFromQueue(ulong channelId, string subredditName);
    }
}
