using System.Linq;
using System.Threading.Tasks;
using UltimateRedditBot.App.Factories.SubRedditFactory;
using UltimateRedditBot.Infra.AppServices;

namespace UltimateRedditBot.App.Factories.SubRedditHistoryFactory
{
    public class SubRedditHistoryFactory : ISubRedditHistoryFactory
    {
        #region Fields

        private readonly ISubRedditFactory _subRedditFactory;
        private readonly ISubRedditHistoryAppService _subRedditHistoryAppService;

        #endregion

        #region Constructors

        public SubRedditHistoryFactory(ISubRedditFactory subRedditFactory, ISubRedditHistoryAppService subRedditHistoryAppService)
        {
            _subRedditFactory = subRedditFactory;
            _subRedditHistoryAppService = subRedditHistoryAppService;
        }

        #endregion

        #region Methods

        public async Task<string> UnSubscribe(ulong guildId, string subredditName)
        {
            var subreddit = await _subRedditFactory.GetSubRedditByName(subredditName);
            if (subreddit is null)
                return "subreddit doesn't exist";

            var historiesTask = await _subRedditHistoryAppService.GetAll();

            var histories = await historiesTask.ToListAsync();
            var history = histories.FirstOrDefault(x => x.SubRedditId == subreddit.Id && x.GuildId == guildId);
            if (history is null)
                return "No history found";

            await _subRedditHistoryAppService.Delete(histories.FirstOrDefault(x => x.SubRedditId == subreddit.Id && x.GuildId == guildId));
            return "Successfully cleared the history";
        }

        #endregion

    }
}
