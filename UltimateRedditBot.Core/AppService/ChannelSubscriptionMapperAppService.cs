using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UltimateRedditBot.Core.Repository;
using UltimateRedditBot.Database;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.AppServices;

namespace UltimateRedditBot.Core.AppService
{
    public class ChannelSubscriptionMapperAppService : Repository<ChannelSubscriptionMapper>, IChannelSubscriptionMapperAppService
    {
        #region Constructor

        public ChannelSubscriptionMapperAppService(Context context, ILogger<Repository.Repository> logger)
            : base(context, logger)
        {

        }

        #endregion


        #region Methods

        public async Task<IEnumerable<ChannelSubscriptionMapper>> GetBySubscriptionId(int subscriptionId)
        {
            return await Queryable().Where(mapper => mapper.SubscriptionId == subscriptionId).ToListAsync();
        }

        public async Task<IEnumerable<ChannelSubscriptionMapper>> GetByChannelId(ulong channelId)
        {
            return await Queryable().Include(x => x.Subscription).ThenInclude(x => x.SubReddit).Where(mapper => mapper.ChannelId == channelId).ToListAsync();
        }

        public async Task<ChannelSubscriptionMapper> GetByChannelIdAndSubscriptionId(ulong channelId, int subscriptionId)
        {
            return await Queryable().FirstOrDefaultAsync(mapper =>
                mapper.SubscriptionId == subscriptionId && mapper.ChannelId == channelId);
        }

        public async Task Unsubscribe(ulong channelId, int subscriptionId)
        {
            var map = await GetByChannelIdAndSubscriptionId(channelId, subscriptionId);

            if(map is not null)
                await Delete(map);
        }

        #endregion


    }
}
