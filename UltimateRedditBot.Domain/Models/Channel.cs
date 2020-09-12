using UltimateRedditBot.Domain.Common;

namespace UltimateRedditBot.Domain.Models
{
    public class Channel : BaseEntity<ulong>
    {
        #region Constructor
        public Channel()
        {

        }

        public Channel(int guildId)
        {
            GuildId = guildId;
        }

        #endregion

        #region Methods

        #endregion

        #region Properties

        public Guild Guild { get; protected set; }
        public int GuildId { get; protected set; }
         
        #endregion
    }
}
