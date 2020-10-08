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
    public class SubRedditHistoryAppService : Repository<SubRedditHistory>, ISubRedditHistoryAppService
    {
        #region Constructor

        public SubRedditHistoryAppService(Context context, ILogger<Repository.Repository> logger)
            : base(context, logger)
        {

        }

        #endregion

        #region Methods

        public async Task RemoveAllGuildHistories(ulong guildId)
        {
            var guildHistories = await Queryable().Where(x => x.GuildId == guildId).ToListAsync();
            if (guildHistories is null)
                return;

            await Delete(guildHistories);
        }

        public async Task<SubRedditHistory> GetByGuildAndSubredditId(ulong guildId, int subredditId)
        {
            return await Queryable().FirstOrDefaultAsync(x => x.GuildId == guildId && x.SubRedditId == subredditId);
        }

        #endregion
    }
}
