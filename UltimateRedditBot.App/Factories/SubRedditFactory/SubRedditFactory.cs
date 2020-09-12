using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.AppServices;
using UltimateRedditBot.Infra.Services;
using UltimateRedditBot.Infra.Uow;

namespace UltimateRedditBot.App.Factories.SubRedditFactory
{
    public class SubRedditFactory : ISubRedditFactory
    {
        #region Fields

        private readonly IRedditApiService _redditApiService;
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Constructor

        public SubRedditFactory(IRedditApiService redditApiService,
            IUnitOfWork unitOfWork)
        {
            _redditApiService = redditApiService;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Methods

        public async Task<SubReddit> GetSubRedditByName(string name)
        {
            var subReddit = await GetFromDatabase(name);

            if (subReddit == null)
                subReddit = await GetFromApi(name);

            return subReddit;
        }

        #region Utils

        private async Task<SubReddit> GetFromDatabase(string name)
        {
            var subReddit = await _unitOfWork.SubRedditRepository.GetSubRedditByName(name);
            return subReddit;
        }

        private async Task<SubReddit> GetFromApi(string name)
        {
            var subReddit = await _redditApiService.GetSubRedditByName(name);
            if (subReddit == null)
                return subReddit;

            await _unitOfWork.SubRedditRepository.Insert(subReddit);
            _unitOfWork.Commit();

            return subReddit;
        }

        #endregion

        #endregion

    }
}
