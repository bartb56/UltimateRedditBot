using System.Threading.Tasks;

namespace UltimateRedditBot.App.Factories.ChannelFactory
{
    public interface IChannelFactory
    {
        Task AddIfNotExisting(ulong id);
    }
}
