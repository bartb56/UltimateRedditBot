using System;
using System.Linq;
using System.Threading.Tasks;
using UltimateRedditBot.App.Constants;
using UltimateRedditBot.Infra.Uow;

namespace UltimateRedditBot.App.Factories.GuildSettingsFactory
{
    public class GuildSettingsFactory : IGuildSettingsFactory
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private static readonly string[] _allowedKeys = { DefaultSettingKeys.Bulk };

        #endregion

        #region Constructor

        public GuildSettingsFactory(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> GetGuildSettingByKey(ulong guildId, string key)
        {
            return await GetGuildSettingByKey<string>(guildId, key);
        }

        public async Task<TObj> GetGuildSettingByKey<TObj>(ulong guildId, string key)
        {
            return await _unitOfWork.GuildSettingsRepository.GetGuildSettingByKey<TObj>(guildId, key);
        }

        public async Task<bool> SaveChanges(ulong guildId, string key, string value)
        {
            return await SaveChanges<string>(guildId, key, value);
        }

        public async Task<bool> SaveChanges<TObj>(ulong guildId, string key, TObj value)
        {
            if (_allowedKeys.FirstOrDefault(allowedKey => allowedKey.Equals(key, StringComparison.OrdinalIgnoreCase)) != null)
            {
                await _unitOfWork.GuildSettingsRepository.SaveChanges<TObj>(guildId, key, value);
                return true;
            }
            return false;
        }

        #endregion

        #region Methods



        #endregion


    }
}
