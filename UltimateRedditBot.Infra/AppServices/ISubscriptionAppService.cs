using System.Collections.Generic;
using System.Threading.Tasks;
using UltimateRedditBot.Domain.Enums;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.Repository;

namespace UltimateRedditBot.Infra.AppServices
{
    public interface ISubscriptionAppService : IRepository<Subscription>
    {
        Task<IEnumerable<Subscription>> GetAllIncluding();

        Task<Subscription> AddIfNotExisting(int subRedditId, Sort sort);

        Task<Subscription> GetBySubRedditAndChannelId(int subRedditId, ulong channelId);
    }
}
