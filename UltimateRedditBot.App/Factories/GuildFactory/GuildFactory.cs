using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.Uow;

namespace UltimateRedditBot.App.Factories.GuildFactory
{
    public class GuildFactory : IGuildFactory
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Constructor

        public GuildFactory(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Methods

        public async Task Insert(IEnumerable<ulong> guildIds)
        {
            var guilds = await _unitOfWork.GuildRepository.Get();

            guildIds = guildIds.Where(guildId => guilds.FirstOrDefault(x => x.Id == guildId) == null);
            if (guildIds.Any())
                await _unitOfWork.GuildRepository.Insert(guildIds.Select(id => new Guild(id)));

            _unitOfWork.Commit();
        }

        public async Task Insert(ulong guildId)
        {
            await _unitOfWork.GuildRepository.Insert(new Guild(guildId));

            _unitOfWork.Commit();
        }


        #endregion
    }
}
