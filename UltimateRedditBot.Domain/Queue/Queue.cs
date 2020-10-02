using System;
using System.Collections.Generic;

namespace UltimateRedditBot.Domain.Queue
{
    public class Queue
    {
        public Queue()
        {
        }

        public bool HasNewQueueItems { get; set; }

        public List<QueueItem> QueueItems { get; set; }
    }
}
