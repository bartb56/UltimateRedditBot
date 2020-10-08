using Microsoft.Extensions.Logging;
using UltimateRedditBot.Core.Repository;
using UltimateRedditBot.Database;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.AppServices;

namespace UltimateRedditBot.Core.AppService
{
    public class PostAppService : Repository<Post, string>, IPostAppService
    {
        public PostAppService(Context context, ILogger<Repository.Repository> logger)
            : base(context, logger)
        {

        }
    }
}
