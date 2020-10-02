using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UltimateRedditBot.Core.Repository;
using UltimateRedditBot.Database;
using UltimateRedditBot.Domain.Enums;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.AppServices;

namespace UltimateRedditBot.Core.AppService
{
    public class SubscriptionAppService : Repository<Subscription>, ISubscriptionAppService
    {
        #region Constructor

        public SubscriptionAppService(Context context, ILogger<Repository.Repository> logger)
            : base(context, logger)
        {
        }

        #endregion

        #region Methods

        public async Task<IEnumerable<Subscription>> GetAllIncluding()
        {
            return await Queryable().Include(x => x.SubReddit).Include(x => x.ChannelSubscriptionMappers)
                .ThenInclude(x => x.Channel).ToListAsync();
        }

        public async Task<Subscription> AddIfNotExisting(int subRedditId, Sort sort)
        {
            var subscription = await GetBySubRedditIdAndSort(subRedditId, sort);
            if (subscription is not null)
                return subscription;

            subscription = new Subscription(subRedditId, "", sort);
            await Insert(subscription);
            return subscription;
        }

        public async Task<Subscription> GetBySubRedditAndChannelId(int subRedditId, ulong channelId)
        {
            var subscriptions = await GetBySubRedditId(subRedditId);

            var subscription = subscriptions.FirstOrDefault(
                x => x.ChannelSubscriptionMappers.FirstOrDefault(y => y.ChannelId == channelId) != null);

            return subscription ?? subscriptions.FirstOrDefault();
        }

        public async Task<Subscription> GetBySubRedditIdAndSort(int subRedditId, Sort sort)
        {
            return await Queryable().FirstOrDefaultAsync(subscription =>
                subscription.SubRedditId == subRedditId && subscription.Sort == sort);
        }

        private async Task<IQueryable<Subscription>> GetBySubRedditId(int subRedditId)
        {
            return Queryable().Include(subscription => subscription.ChannelSubscriptionMappers)
                .Where(subscription => subscription.SubRedditId == subRedditId);

        }

        #endregion

    }
}
