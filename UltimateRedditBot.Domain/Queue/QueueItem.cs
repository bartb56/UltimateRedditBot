using Discord;
using System;
using System.Diagnostics.CodeAnalysis;
using UltimateRedditBot.Domain.Models;

namespace UltimateRedditBot.Domain.Queue
{
    [Serializable]
    public class QueueItem : IEquatable<QueueItem>
    {
        #region Constructor

        public QueueItem(ulong guildId, int subRedditId, ulong channelId, PostType postType, Guid id)
        {
            GuildId = guildId;
            SubRedditId = subRedditId;
            ChannelId = channelId;
            PostType = postType;

            Id = id;
        }

        public QueueItem()
        {

        }

        #endregion

        #region Methods

        #endregion

        #region Properties

        public Guid Id { get; set; }

        public ulong GuildId { get; set; }

        public int SubRedditId { get; set; }

        public ulong ChannelId { get; set; }

        public PostType PostType { get; set; }


        public bool Equals([AllowNull] QueueItem other)
        {
            if (other is null)
                return false;

            return this.Id == other.Id && this.GuildId == other.GuildId && this.SubRedditId == other.SubRedditId && this.ChannelId == other.ChannelId && this.PostType == other.PostType;
        }

        #endregion
    }
}
