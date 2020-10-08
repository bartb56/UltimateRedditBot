using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.AppServices;

namespace UltimateRedditBot.App.Factories.GuildFactory
{
    public class GuildFactory : IGuildFactory
    {
        #region Fields

        private readonly IGuildAppService _guildAppService;

        #endregion

        #region Constructor

        public GuildFactory(IGuildAppService guildAppService)
        {
            _guildAppService = guildAppService;
        }

        #endregion

        #region Methods

        public async Task Insert(IEnumerable<ulong> guildIds)
        {
            var guilds = await _guildAppService.Get();
            var existingGuilds = guilds.ToList();

            var allNewGuildIds = guildIds.Where(guildId => existingGuilds.FirstOrDefault(x => x.Id == guildId) is null).ToList();

            if (!allNewGuildIds.Any())
                return;

            await _guildAppService.Insert(allNewGuildIds.Select(id => new Guild(id)));
        }

        public async Task Insert(ulong guildId)
        {
            await _guildAppService.Insert(new Guild(guildId));
        }

        #endregion
    }
}
