using UltimateRedditBot.Core.Repository;
using UltimateRedditBot.Database;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.AppServices;

namespace UltimateRedditBot.Core.AppService
{
    public class SubRedditHistoryAppService : Repository<SubRedditHistory>, ISubRedditHistoryAppService
    {
        public SubRedditHistoryAppService(Context context)
            : base(context)
        {

        }
    }
}
