using System.Collections.Generic;
using UltimateRedditBot.Domain.Common;

namespace UltimateRedditBot.Domain.Models
{
    public sealed class Channel : BaseEntity<ulong>
    {
        #region Constructor

        //Empty ctor for ef core
        public Channel()
        { }

        public Channel(ulong channelId)
        {
            Id = channelId;
        }

        #endregion

        #region Properties

        public IEnumerable<ChannelSubscriptionMapper> ChannelSubscriptionMappers { get; set; }

        #endregion
    }
}
