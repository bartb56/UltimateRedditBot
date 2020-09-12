using System.Threading.Tasks;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Domain.Queue;

namespace UltimateRedditBot.Infra.Services
{
    public interface IRedditApiService
    {
        Task<SubReddit> GetSubRedditByName(string name);

        Task<PostDto> GetPost(QueueItem queueItem, string lastUsedName, string subredditName);
    }
}
