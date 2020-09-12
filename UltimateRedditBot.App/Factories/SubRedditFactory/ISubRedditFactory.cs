using System.Threading.Tasks;
using UltimateRedditBot.Domain.Models;

namespace UltimateRedditBot.App.Factories.SubRedditFactory
{
    public interface ISubRedditFactory
    {
        Task<SubReddit> GetSubRedditByName(string name);

    }
}
