using System;
using System.Collections.Generic;

namespace UltimateRedditBot.Domain.Queue
{
    public class Queue : IDisposable
    {
        public Queue()
        {
            QueueItems = new List<QueueItem>();
        }


        public List<QueueItem> QueueItems { get; set; }

        public void Dispose()
        {
            QueueItems = null;
        }
    }
}
