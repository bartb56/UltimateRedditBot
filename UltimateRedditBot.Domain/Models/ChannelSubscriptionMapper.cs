using System.ComponentModel.DataAnnotations.Schema;
using UltimateRedditBot.Domain.Common;

namespace UltimateRedditBot.Domain.Models
{
    public class ChannelSubscriptionMapper : BaseEntity
    {
        #region Constructors

        //Empty ctor for ef core
        public ChannelSubscriptionMapper()
        { }

        public ChannelSubscriptionMapper(ulong channelId, int subscriptionId)
        {
            ChannelId = channelId;
            SubscriptionId = subscriptionId;
        }

        #endregion

        #region Properties

        public Channel Channel { get; set; }
        public ulong ChannelId { get; set; }

        public Subscription Subscription { get; set; }
        public int SubscriptionId { get; set; }

        [NotMapped]
        public override int Id { get; set; }

        #endregion
    }
}
