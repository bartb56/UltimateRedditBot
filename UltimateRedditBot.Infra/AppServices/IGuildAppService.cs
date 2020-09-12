using System.Collections.Generic;
using System.Threading.Tasks;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.Repository;

namespace UltimateRedditBot.Infra.AppServices
{
    public interface IGuildAppService : IRepository<Guild, ulong>
    {
        Task<Guild> GetByGuildId(ulong id);

        Task Insert(IEnumerable<ulong> ids);
    }
}
