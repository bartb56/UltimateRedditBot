using System;

namespace UltimateRedditBot.Core.Services
{
    public interface IQueueGuildService : IDisposable
    {
        void ProccessQueue(ulong guildId);
    }
}
