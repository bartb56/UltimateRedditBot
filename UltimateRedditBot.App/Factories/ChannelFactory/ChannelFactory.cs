using System.Threading.Tasks;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.AppServices;

namespace UltimateRedditBot.App.Factories.ChannelFactory
{
    public class ChannelFactory : IChannelFactory
    {
        #region Fields

        private readonly IChannelAppService _channelAppService;

        #endregion

        #region Constructor

        public ChannelFactory(IChannelAppService channelAppService)
        {
            _channelAppService = channelAppService;
        }

        #endregion

        #region Methods

        public async Task AddIfNotExisting(ulong id)
        {
            var channel = await _channelAppService.GetById(id);

            if (channel is not null)
                return;

            await _channelAppService.Insert(new Channel(id));
        }

        #endregion

    }
}
