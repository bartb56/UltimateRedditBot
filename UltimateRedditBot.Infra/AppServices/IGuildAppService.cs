using System.Collections.Generic;
using System.Threading.Tasks;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.Repository;

namespace UltimateRedditBot.Infra.AppServices
{
    public interface IGuildAppService : IRepository<Guild>
    {
        Task<Guild> GetByGuildId(ulong guildId);

        Task Insert(IEnumerable<ulong> guildIds);
    }
}
