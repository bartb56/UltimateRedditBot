using System;

namespace UltimateRedditBot.Core.Services
{
    public class QueueGuildService : IQueueGuildService
    {
        #region Fields

        private ulong GuildId;

        #endregion

        #region Constructor

        public QueueGuildService()
        {

        }

        #endregion

        #region Methods

        public void Dispose()
        {

        }

        public void ProccessQueue(ulong guildId)
        {
            GuildId = guildId;
        }

        #endregion


    }
}
