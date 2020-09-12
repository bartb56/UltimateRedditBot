using System;
using System.Collections;
using System.Collections.Generic;
using UltimateRedditBot.Domain.Common;

namespace UltimateRedditBot.Domain.Models
{
    [Serializable]
    public class SubReddit : BaseEntity
    {

        #region Constructor

        public SubReddit()
        {

        }

        public SubReddit(string name, bool isNsfw)
        {
            Name = name;
            IsNsfw = isNsfw;

            Posts = new List<Post>();
            SubRedditHistories = new List<SubRedditHistory>();
        }

        #endregion

        #region Methods

        public void Update(string name, bool isNsfw)
        {
            Name = name;
            IsNsfw = isNsfw;
        }

        #endregion

        #region Properties

        public string Name { get; protected set; }

        public bool IsNsfw { get; protected set; }

        public IEnumerable<Post> Posts { get; set; }

        public IEnumerable<SubRedditHistory> SubRedditHistories { get; set; }

        #endregion

    }
}
