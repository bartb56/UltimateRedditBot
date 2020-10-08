using UltimateRedditBot.Domain.Common;

namespace UltimateRedditBot.Domain.Models
{
    public class GuildSettings : BaseEntity
    {
        #region Constructors

        //Empty constructor for entity framework
        public GuildSettings()
        { }


        #endregion

        #region Properties

        public Guild Guild { get; set; }
        public ulong GuildId { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }

        #endregion
    }
}
