using System.Threading.Tasks;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.AppServices;
using UltimateRedditBot.Infra.Services;
namespace UltimateRedditBot.App.Factories.SubRedditFactory
{
    public class SubRedditFactory : ISubRedditFactory
    {
        #region Fields

        private readonly IRedditApiService _redditApiService;
        private readonly ISubRedditAppService _subRedditAppService;

        #endregion

        #region Constructor

        public SubRedditFactory(IRedditApiService redditApiService,
            ISubRedditAppService subRedditAppService)
        {
            _redditApiService = redditApiService;
            _subRedditAppService = subRedditAppService;
        }

        #endregion

        #region Methods

        public async Task<SubReddit> GetSubRedditByName(string name)
        {
            var subReddit = await GetFromDatabase(name);
            return subReddit ?? await GetFromApi(name);
        }

        public async Task<SubReddit> GetById(int id)
        {
            var subreddit = await _subRedditAppService.GetById(id);
            return subreddit;
        }

        #region Utils

        private async Task<SubReddit> GetFromDatabase(string name)
        {
            var subReddit = await _subRedditAppService.GetSubRedditByName(name);
            return subReddit;
        }

        private async Task<SubReddit> GetFromApi(string name)
        {
            var subReddit = await _redditApiService.GetSubRedditByName(name);
            if (subReddit is null)
                return null;

            await _subRedditAppService.Insert(subReddit);
            return subReddit;
        }

        #endregion

        #endregion

    }
}
