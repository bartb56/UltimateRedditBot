using UltimateRedditBot.Domain.Common;

namespace UltimateRedditBot.Domain.Models
{
    public class Channel : BaseEntity
    {
        #region Constructor
        public Channel()
        {

        }

        public Channel(ulong channelId, int guildId)
        {
            ChannelId = channelId;
            GuildId = guildId;
        }

        #endregion

        #region Methods

        #endregion

        #region Properties

        public ulong ChannelId { get; protected set; }

        public Guild Guild { get; protected set; }
        public int GuildId { get; protected set; }
         
        #endregion
    }
}
