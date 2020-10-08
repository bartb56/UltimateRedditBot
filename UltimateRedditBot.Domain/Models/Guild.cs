using System;
using System.Collections.Generic;
using UltimateRedditBot.Domain.Common;

namespace UltimateRedditBot.Domain.Models
{
    [Serializable]
    public sealed class Guild : BaseEntity<ulong>
    {
        #region Contructor

        //Empty ctor for ef core
        public Guild()
        { }

        public Guild(ulong guildId)
        {
            Id = guildId;
            SubRedditHistories = new List<SubRedditHistory>();
        }

        #endregion

        #region Properties

        public IEnumerable<SubRedditHistory> SubRedditHistories { get; set; }

        #endregion
    }
}
