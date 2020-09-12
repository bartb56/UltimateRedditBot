using UltimateRedditBot.Domain.Common;

namespace UltimateRedditBot.Domain.Models
{
    public class GuildSettings : BaseEntity
    {
        public Guild Guild { get; set; }
        public ulong GuildId { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }

    }
}
