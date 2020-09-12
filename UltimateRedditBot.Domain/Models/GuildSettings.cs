using UltimateRedditBot.Domain.Common;

namespace UltimateRedditBot.Domain.Models
{
    public class GuildSettings : BaseEntity
    {
        public Guild Guild { get; set; }
        public int GuildId { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }

    }
}
