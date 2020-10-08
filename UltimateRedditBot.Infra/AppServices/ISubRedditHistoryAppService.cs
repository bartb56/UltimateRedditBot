using System.Threading.Tasks;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.Repository;

namespace UltimateRedditBot.Infra.AppServices
{
    public interface ISubRedditHistoryAppService : IRepository<SubRedditHistory>
    {
        Task RemoveAllGuildHistories(ulong guildId);
    }
}
