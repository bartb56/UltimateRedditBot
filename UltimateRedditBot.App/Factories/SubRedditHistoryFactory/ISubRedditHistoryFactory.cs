using System.Threading.Tasks;

namespace UltimateRedditBot.App.Factories.SubRedditHistoryFactory
{
    public interface ISubRedditHistoryFactory
    {
        Task<string> UnSubscribe(ulong guildId, string subredditName);
    }
}
