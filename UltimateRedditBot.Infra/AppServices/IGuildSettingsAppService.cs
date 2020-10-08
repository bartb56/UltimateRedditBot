using System.Threading.Tasks;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.Repository;

namespace UltimateRedditBot.Infra.AppServices
{
    public interface IGuildSettingsAppService : IRepository<GuildSettings>
    {
        Task SaveChanges<TObj>(ulong guildId, string key, TObj value);

        Task<TObj> GetGuildSettingByKey<TObj>(ulong guildId, string key);
    }
}
