using System.Collections.Generic;
using System.Threading.Tasks;
using UltimateRedditBot.Domain.Enums;
using UltimateRedditBot.Domain.Models;

namespace UltimateRedditBot.App.Factories.Subscriptions
{
    public interface ISubscriptionsFactory
    {
        Task<IEnumerable<Subscription>> Subscriptions(ulong channelId);

        Task Subscribe(ulong channelId, string subRedditName, Sort sort);

        Task Unsubscribe(ulong channelId, string subRedditName);
    }
}
