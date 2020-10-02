using System;
using System.Linq;
using System.Threading.Tasks;
using UltimateRedditBot.App.Constants;
using UltimateRedditBot.Infra.AppServices;

namespace UltimateRedditBot.App.Factories.GuildSettingsFactory
{
    public class GuildSettingsFactory : IGuildSettingsFactory
    {
        #region Fields

        private readonly IGuildSettingsAppService _guildSettingsAppService;
        private readonly ISubRedditHistoryAppService _redditHistoryAppService;
        private static readonly string[] AllowedKeys = { DefaultSettingKeys.Bulk, DefaultSettingKeys.Sort };

        #endregion

        #region Constructor

        public GuildSettingsFactory(IGuildSettingsAppService guildSettingsAppService, ISubRedditHistoryAppService redditHistoryAppService)
        {
            _guildSettingsAppService = guildSettingsAppService;
            _redditHistoryAppService = redditHistoryAppService;
        }

        #endregion

        #region Methods

        public async Task<string> GetGuildSettingByKey(ulong guildId, string key)
        {
            return await GetGuildSettingByKey<string>(guildId, key);
        }

        public async Task<TObj> GetGuildSettingByKey<TObj>(ulong guildId, string key)
        {
            return await _guildSettingsAppService.GetGuildSettingByKey<TObj>(guildId, key);
        }

        public async Task<bool> SaveChanges(ulong guildId, string key, string value)
        {
            if (key.Equals(DefaultSettingKeys.Sort))
                await _redditHistoryAppService.RemoveAllGuildHistories(guildId);

            return await SaveChanges<string>(guildId, key, value);
        }

        public async Task<bool> SaveChanges<TObj>(ulong guildId, string key, TObj value)
        {
            if (AllowedKeys.FirstOrDefault(allowedKey => allowedKey.Equals(key, StringComparison.OrdinalIgnoreCase)) ==
                null) return false;

            await _guildSettingsAppService.SaveChanges(guildId, key, value);
            return true;
        }

        #endregion

    }
}
