using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateRedditBot.Domain.Models
{
    public class PostDto
    {
        public Post Post { get; set; }

        public Guid QueueItemId { get; set; }
    }
}
