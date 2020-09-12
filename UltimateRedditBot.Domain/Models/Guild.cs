using System;
using System.Collections.Generic;
using UltimateRedditBot.Domain.Common;

namespace UltimateRedditBot.Domain.Models
{
    [Serializable]
    public class Guild : BaseEntity<ulong>
    {
        #region Contructor

        public Guild()
        {

        }

        public Guild(ulong guildId)
        {
            Id = guildId;
            SubRedditHistories = new List<SubRedditHistory>();
            Channels = new List<Channel>();
        }

        #endregion

        #region Properties

        public IEnumerable<SubRedditHistory> SubRedditHistories { get; set; }

        public IEnumerable<Channel> Channels { get; set; }

        #endregion
    }
}
