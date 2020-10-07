using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.Repository;

namespace UltimateRedditBot.Infra.AppServices
{
    public interface IPostAppService : IRepository<Post, string>
    {
    }
}
