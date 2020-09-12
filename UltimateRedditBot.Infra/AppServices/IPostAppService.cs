using System.Threading.Tasks;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.Repository;

namespace UltimateRedditBot.Infra.AppServices
{
    public interface IPostAppService : IRepository<Post>
    {
        Task<bool> IsUniquePost(Post post);

        Task<Post> GetNextPost(int previousPostId, int subRedditId);

        Task<Post> GetByPostId(string postId);
    }
}
