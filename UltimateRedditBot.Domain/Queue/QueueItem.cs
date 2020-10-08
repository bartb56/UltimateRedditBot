using System;
using UltimateRedditBot.Domain.Enums;
using UltimateRedditBot.Domain.Models;

namespace UltimateRedditBot.Domain.Queue
{
    [Serializable]
    public class QueueItem
    {
        #region Constructor

        public QueueItem(int subRedditId, string subredditName, ulong channelId, PostType postType, Guid id, Sort sort)
        {
            SubRedditId = subRedditId;
            SubredditName = subredditName;
            ChannelId = channelId;
            PostType = postType;
            Id = id;
            Sort = sort;
        }

        public QueueItem()
        {

        }

        #endregion

        #region Methods

        #endregion

        #region Properties

        public Guid Id { get; set; }

        public int SubRedditId { get; set; }

        public string SubredditName { get; set; }

        public ulong ChannelId { get; set; }

        public PostType PostType { get; set; }

        public Sort Sort { get; set; }

        #endregion
    }
}
