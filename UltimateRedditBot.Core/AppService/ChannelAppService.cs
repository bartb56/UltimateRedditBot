using Microsoft.Extensions.Logging;
using UltimateRedditBot.Core.Repository;
using UltimateRedditBot.Database;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.AppServices;

namespace UltimateRedditBot.Core.AppService
{
    public class ChannelAppService : Repository<Channel, ulong>, IChannelAppService
    {
        public ChannelAppService(Context context, ILogger<Repository.Repository> logger)
            : base(context, logger)
        {

        }
    }
}
