using System.Collections.Generic;
using System.Threading.Tasks;

namespace UltimateRedditBot.App.Factories.GuildFactory
{
    public interface IGuildFactory
    {
        Task Insert(IEnumerable<ulong> guildIds);

        Task Insert(ulong guildId);
    }
}
