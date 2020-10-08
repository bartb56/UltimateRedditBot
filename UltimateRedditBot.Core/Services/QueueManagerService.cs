using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;
using UltimateRedditBot.Domain.Queue;
using UltimateRedditBot.Infra.Services;

namespace UltimateRedditBot.Core.Services
{
    public class QueueManagerService : IQueueManagerService
    {
        #region Fields

        private readonly IQueueService _queueService;
        private readonly DiscordSocketClient _discordClient;
        private readonly List<IQueueClient> _queueClients = new List<IQueueClient>();

        #endregion

        #region Constructors

        public QueueManagerService(IQueueService queueService, DiscordSocketClient discordClient)
        {
            _queueService = queueService;
            _discordClient = discordClient;
        }

        #endregion

        #region Methods

        public void AddToQueue(IEnumerable<QueueItem> queueItems, ulong guildId)
        {
            var client = _queueClients.FirstOrDefault(c => c.GuildId == guildId);

            //If the client doesn't exist, create a new one.
            var isNewClient = client is null;
            client ??= new QueueClient(guildId, _queueService);

            client.QueueItems = client.QueueItems?.Concat(queueItems);
            var queueClient = client as QueueClient;

            if (!isNewClient)
                return;

#pragma warning disable 4014
            queueClient.Start();
#pragma warning restore 4014
            _queueClients.Add(queueClient);
        }

        public IEnumerable<QueueItem> GetGuildQueueItems(ulong guildId)
        {
            var client = _queueClients.FirstOrDefault(x => x.GuildId == guildId);
            return client?.QueueItems;
        }

        public void RemoveByChannelId(ulong channelId)
        {
            var client = GetQueueClientFromChannelId(channelId);
            client?.RemoveByChannelId(channelId);
        }

        public void RemoveSubredditFromQueue(ulong channelId, int subredditId)
        {
            var client = GetQueueClientFromChannelId(channelId);
            client.RemoveBySubredditId(channelId, subredditId);
        }

        private IQueueClient GetQueueClient(ulong guildId)
        {
            return _queueClients.FirstOrDefault(client => client.GuildId == guildId);
        }

        private IQueueClient GetQueueClientFromChannelId(ulong channelId)
        {
            var guildId = GetGuildIdFromChannelId(channelId);
            return GetQueueClient(guildId);
        }

        private ulong GetGuildIdFromChannelId(ulong channelId)
        {
            var channel = _discordClient.GetChannel(channelId) as ITextChannel;
            return channel.GuildId;
        }

        #endregion

    }
}
