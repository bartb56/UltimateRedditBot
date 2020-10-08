using System.Collections.Generic;
using System.Threading.Tasks;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.Repository;

namespace UltimateRedditBot.Infra.AppServices
{
    public interface IChannelSubscriptionMapperAppService : IRepository<ChannelSubscriptionMapper>
    {
        Task<IEnumerable<ChannelSubscriptionMapper>> GetByChannelId(ulong channelId);

        Task<ChannelSubscriptionMapper> GetByChannelIdAndSubscriptionId(ulong channelId, int subscriptionId);


        Task Unsubscribe(ulong channelId, int subscriptionId);
    }
}
