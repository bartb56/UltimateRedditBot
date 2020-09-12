using UltimateRedditBot.Domain.Common;

namespace UltimateRedditBot.Domain.Models
{
    public class SubRedditHistory : BaseEntity
    {
        #region Constructor

        public SubRedditHistory()
        {

        }

        public SubRedditHistory(int postId, ulong guildId, int subRedditId)
        {
            LastPostId = postId;
            GuildId = guildId;
            SubRedditId = subRedditId;
        }

        #endregion

        #region Methods

        public void UpdateLastPostId(int postId)
        {
            LastPostId = postId;
        }

        #endregion

        #region Properties

        public Post LastPost { get; set; }
        public int LastPostId { get; protected set; }

        public virtual Guild Guild { get; set; }
        public ulong GuildId { get; protected set; }


        public virtual SubReddit SubReddit { get; set; }
        public int SubRedditId { get; protected set; }

        #endregion
    }
}
