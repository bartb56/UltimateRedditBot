using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UltimateRedditBot.Core.Extensions;
using UltimateRedditBot.Domain.Queue;
using UltimateRedditBot.Infra.Services;

namespace UltimateRedditBot.Core.Services
{
    [Serializable]
    public class QueueClient : IQueueClient
    {
        #region Fields

        public ulong GuildId { get; private set; }
        public IEnumerable<QueueItem> QueueItems { get; set; }
        private readonly IQueueService _queueService;

        #endregion

        #region Consturctor

        public QueueClient(ulong guildId, IQueueService queueService)
        {
            GuildId = guildId;
            _queueService = queueService;
            QueueItems = new List<QueueItem>();
        }

        #endregion

        #region Methods

        public async Task Start()
        {
            while (true)
            {
                await Task.Run(async () =>
                {
                    if (!QueueItems.Any())
                        return;

                    var queueItems = QueueItems.DistinctBy(x => x.SubRedditId).ToList();

                    await Process(queueItems);
                });
            }
        }

        public void RemoveByChannelId(ulong channelId)
        {
            QueueItems = QueueItems?.Where(x => x.ChannelId != channelId).ToList();
        }

        public void RemoveBySubredditId(ulong channelId, int subredditId)
        {
            QueueItems = QueueItems.Where(x => x.ChannelId != channelId || x.SubRedditId != subredditId ).ToList();
        }

        #region Utils

        private async Task Process(ICollection<QueueItem> queueItems)
        {
            if (!queueItems.Any())
                return;

            await _queueService.ProcessQueue(queueItems.ToAsyncEnumerable());
            lock (QueueItems)
            {
                var queue = QueueItems.ToList();
                queue.RemoveAll(queueItems.Contains);
                QueueItems = queue;
            }
        }

        #endregion

        #endregion
    }
}
