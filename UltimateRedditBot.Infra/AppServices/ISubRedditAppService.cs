using System.Threading.Tasks;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.Repository;

namespace UltimateRedditBot.Infra.AppServices
{
    public interface ISubRedditAppService : IRepository<SubReddit>
    {
        Task<SubReddit> GetSubRedditByName(string name);
    }
}
