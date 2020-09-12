using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UltimateRedditBot.Core.Repository;
using UltimateRedditBot.Database;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.AppServices;

namespace UltimateRedditBot.Core.AppService
{
    public class GuildAppService : Repository<Guild>, IGuildAppService
    {
        public GuildAppService(Context context)
            : base(context)
        {

        }

        public async Task<Guild> GetByGuildId(ulong guildId)
        {
            return await Queriable().FirstOrDefaultAsync(guild => guild.GuildId == guildId);
        }

        public async Task Insert(IEnumerable<ulong> guildIds)
        {
            var guilds = guildIds.Select(guildId => new Guild(guildId)).ToList();
            await Insert(guilds);
        }
    }
}
