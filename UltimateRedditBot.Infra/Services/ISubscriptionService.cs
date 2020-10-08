using System.Threading.Tasks;

namespace UltimateRedditBot.Infra.Services
{
    public interface ISubscriptionService
    {
        Task HandleSubscriptions();
    }
}
