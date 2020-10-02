﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UltimateRedditBot.Core.Repository;
using UltimateRedditBot.Database;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.AppServices;

namespace UltimateRedditBot.Core.AppService
{
    public class GuildAppService : Repository<Guild, ulong>, IGuildAppService
    {
        public GuildAppService(Context context, ILogger<Repository.Repository> logger)
            : base(context, logger)
        {

        }

        public async Task<Guild> GetByGuildId(ulong id)
        {
            return await Queryable().FirstOrDefaultAsync(guild => guild.Id == id);
        }

        public async Task Insert(IEnumerable<ulong> ids)
        {
            var guilds = ids.Select(guildId => new Guild(guildId)).ToList();
            await Insert(guilds);
        }
    }
}
