using System.Threading.Tasks;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.Repository;

namespace UltimateRedditBot.Infra.AppServices
{
    public interface IGuildSettingsAppService : IRepository<GuildSettings>
    {
        Task<GuildSettings> GetByGuildId(ulong guildId);

        Task SaveChanges(ulong guildId, string key, string value);

        Task SaveChanges<TObj>(ulong guildId, string key, TObj value);

        Task<string> GetGuildSettingByKey(ulong guildId, string key);

        Task<TObj> GetGuildSettingByKey<TObj>(ulong guildId, string key);
    }
}
