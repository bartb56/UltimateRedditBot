using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using UltimateRedditBot.Core.Repository;
using UltimateRedditBot.Database;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.AppServices;

namespace UltimateRedditBot.Core.AppService
{
    public class PostAppService : Repository<Post>, IPostAppService
    {
        public PostAppService(Context context)
            : base(context)
        {

        }

        public async Task<Post> GetByPostId(string postId)
        {
            return await Queriable().FirstOrDefaultAsync(post => post.PostId == postId);
        }

        public async Task<Post> GetNextPost(int previousPostId, int subRedditId)
        {
            return await Queriable().FirstOrDefaultAsync(post => post.Id > previousPostId && post.SubRedditId == subRedditId);
        }

        public async Task<bool> IsUniquePost(Post newPost)
        {
            return await Queriable().FirstOrDefaultAsync(post => post.PostId == newPost.PostId) is null;
        }
    }
}
