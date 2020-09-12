using UltimateRedditBot.Infra.AppServices;

namespace UltimateRedditBot.Infra.Uow
{
    public interface IUnitOfWork
    {
        ISubRedditAppService SubRedditRepository { get; }

        ISubRedditHistoryAppService SubRedditHistoryRepository { get; }

        IPostAppService PostRepository { get; }

        IGuildSettingsAppService GuildSettingsRepository { get;  }

        IGuildAppService GuildRepository { get; }

        void Commit();
    }
}
