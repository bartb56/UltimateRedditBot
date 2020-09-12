using System;
using System.Collections.Generic;
using UltimateRedditBot.Domain.Common;

namespace UltimateRedditBot.Domain.Models
{
    [Serializable]
    public class Guild : BaseEntity
    {
        #region Contructor

        public Guild()
        {

        }

        public Guild(ulong guildId)
        {
            GuildId = guildId;

            SubRedditHistories = new List<SubRedditHistory>();
            Channels = new List<Channel>();
        }

        #endregion

        #region Methods

        public void UpdateGuildId(ulong guildId)
        {
            GuildId = guildId;
        }

        #endregion

        #region Properties

        public ulong GuildId { get; protected set; }

        public IEnumerable<SubRedditHistory> SubRedditHistories { get; set; }

        public IEnumerable<Channel> Channels { get; set; }

        #endregion
    }
}
