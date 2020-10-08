using System;
using System.Collections.Generic;
using UltimateRedditBot.Domain.Common;
using UltimateRedditBot.Domain.Enums;

namespace UltimateRedditBot.Domain.Models
{
    [Serializable]
    public class SubReddit : BaseEntity
    {

        #region Constructor

        //Empty ctor for ef core
        public SubReddit()
        { }

        public SubReddit(string name, bool isNsfw)
        {
            Name = name;
            IsNsfw = isNsfw;

            Posts = new List<Post>();
            SubRedditHistories = new List<SubRedditHistory>();
        }

        #endregion

        #region Methods

        public void Update(string name, bool isNsfw, Sort sort)
        {
            Name = name;
            IsNsfw = isNsfw;
            Sort = sort;
        }

        #endregion

        #region Properties

        public string Name { get; protected set; }

        public bool IsNsfw { get; protected set; }

        public Sort Sort { get; protected set; }

        public IEnumerable<Post> Posts { get; set; }

        public IEnumerable<SubRedditHistory> SubRedditHistories { get; set; }

        #endregion

    }
}
