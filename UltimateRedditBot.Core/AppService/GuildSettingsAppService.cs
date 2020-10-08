using Microsoft.EntityFrameworkCore;
using System;
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
    public class GuildSettingsAppService : Repository<GuildSettings>, IGuildSettingsAppService
    {
        #region Fields

        private readonly IGuildAppService _guildAppService;

        #endregion

        #region Constructor

        public GuildSettingsAppService(Context context, IGuildAppService guildAppService, ILogger<Repository.Repository> logger)
            : base(context, logger)
        {
            _guildAppService = guildAppService;
        }

        #endregion

        #region Methods

        public async Task<string> GetGuildSettingByKey(ulong guildId, string key)
        {
            return await GetGuildSettingByKey<string>(guildId, key);
        }

        public async Task<TObj> GetGuildSettingByKey<TObj>(ulong guildId, string key)
        {
            var settings = await GetGuildBySettings(guildId);

            var setting = settings?.FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (setting is null)
                return default;

            if (typeof(TObj).BaseType == typeof(Enum))
            {
                return (TObj)Enum.Parse(typeof(TObj), setting.Value, true);
            }

            return (TObj)Convert.ChangeType(setting.Value, typeof(TObj));
        }

        public async Task SaveChanges(ulong guildId, string key, string value)
        {
            await SaveChanges<string>(guildId, key, value);
        }

        public async Task SaveChanges<TObj>(ulong guildId, string key, TObj value)
        {
            var setting = await GetGuildSetting(guildId, key);

            switch (setting)
            {
                case null when value is not null:
                    setting = new GuildSettings
                    {
                        GuildId = guildId,
                        Key = key,
                        Value = value.ToString()
                    };

                    await Insert(setting);
                    break;
                case not null when value is not null:
                    setting.Value = value.ToString();
                    await Update(setting);
                    break;
                case not null when true:
                    await Delete(setting);
                    break;
            }
        }

        private async Task<GuildSettings> GetGuildSetting(ulong guildId, string key)
        {
            var guildSettings = await GetGuildBySettings(guildId);
            return guildSettings?.FirstOrDefault(setting => setting.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<IEnumerable<GuildSettings>> GetGuildBySettings(ulong guildId)
        {
            var guild = await _guildAppService.GetByGuildId(guildId);

            return Queryable()?.Where(setting => setting.GuildId == guild.Id).AsQueryable();
        }

        #endregion

        public async Task<GuildSettings> GetByGuildId(ulong guildId)
        {
            return await Queryable().FirstOrDefaultAsync(guildSettings => guildSettings.GuildId == guildId);
        }
    }
}
