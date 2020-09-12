using System.Threading.Tasks;

namespace UltimateRedditBot.App.Factories.GuildSettingsFactory
{
    public interface IGuildSettingsFactory
    {
        Task<bool> SaveChanges(ulong guildId, string key, string value);

        Task<bool> SaveChanges<TObj>(ulong guildId, string key, TObj value);

        Task<string> GetGuildSettingByKey(ulong guildId, string key);

        Task<TObj> GetGuildSettingByKey<TObj>(ulong guildId, string key);
    }
}
