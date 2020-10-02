using System;

namespace UltimateRedditBot.Domain.Models
{
    public class PostDto
    {
        public Post Post { get; set; }

        public Guid QueueItemId { get; set; }
    }
}
