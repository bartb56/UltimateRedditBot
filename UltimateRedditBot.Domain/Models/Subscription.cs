using System.Collections.Generic;
using UltimateRedditBot.Domain.Common;
using UltimateRedditBot.Domain.Enums;

namespace UltimateRedditBot.Domain.Models
{
    public class Subscription : BaseEntity
    {
        #region Constructor

        //Empty constructor for entity framework
        public Subscription()
        { }

        public Subscription(int subRedditId, string lastPostName, Sort sort)
        {
            SubRedditId = subRedditId;
            LastPostName = lastPostName;
            Sort = sort;
        }

        #endregion

        #region Methods

        public void UpdateLastPostName(string postName)
        {
            LastPostName = postName;
        }

        #endregion

        #region Properties

        public SubReddit SubReddit { get; set; }
        public int SubRedditId { get; private set; }

        public string LastPostName { get; private set; }

        public Sort Sort { get; private set; }

        public IEnumerable<ChannelSubscriptionMapper> ChannelSubscriptionMappers { get; set; }

        #endregion
    }
}
