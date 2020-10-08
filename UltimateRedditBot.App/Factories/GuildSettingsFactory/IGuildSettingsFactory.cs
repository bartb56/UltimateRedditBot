using System.Threading.Tasks;

namespace UltimateRedditBot.App.Factories.GuildSettingsFactory
{
    public interface IGuildSettingsFactory
    {
        Task<bool> SaveChanges(ulong guildId, string key, string value);

        Task<TObj> GetGuildSettingByKey<TObj>(ulong guildId, string key);
    }
}
