using System;
using System.ComponentModel.DataAnnotations;
using Discord;
using UltimateRedditBot.Domain.Common;

namespace UltimateRedditBot.Domain.Models
{
    public class Post : BaseEntity<string>
    {
        #region Constructor

        //Emptu ctore for ef core
        public Post()
        { }

        public Post(string postId, string author, int downs, int ups, bool isOver18, string title, string postLink, Uri thumbnail, string selfText, Uri url, PostType postType)
        {
            Id = postId;
            Author = author;
            Downs = downs;
            Ups = ups;
            IsOver18 = isOver18;
            Title = title;
            PostLink = postLink;
            Thumbnail = thumbnail;
            Selftext = selfText;
            Url = url;
            PostType = postType;
        }

        #endregion

        #region Methods

        public PostType GetPostType()
        {
            if (Url is null)
                return PostType;

            var url = Url.ToString();

            if (url.Contains(".gif") || url.Contains("https://gfycat") || url.Contains("https://redgifs"))
                PostType = PostType.Gif;

            if (url.Contains(".jpg") || url.Contains(".png") || url.Contains(".jpeg"))
                PostType = PostType.Gif;

            if (url.Contains(".mp4"))
                PostType = PostType.Video;

            return PostType;
        }

        public Embed Embed(string subreddit)
        {
            var thumbsUp = new Emoji("\uD83D\uDC4D");
            var footer = new EmbedFooterBuilder
            {
                Text = $"Posted by: u/{Author} \nPosted in: r/{subreddit} \n👍 {Ups}"
            };

            var title = Title;
            if (title.Length > 256)
                title = title.Substring(256);

            var embedBuilder = new EmbedBuilder
            {
                Color = Color.Gold,
                Title = title,
                ImageUrl = Url.ToString(),
                Url = $"https://reddit.com{PostLink}",
                Footer = footer
            };

            return embedBuilder.Build();
        }

        public void UpdateUpsAndDowns(int downs, int ups)
        {
            Downs = downs;
            Ups = ups;
        }

        #endregion

        #region Properties

        public string Author { get; set; }

        public int Downs { get; set; }

        public int Ups { get; set; }

        public bool IsOver18 { get; set; }

        public string Title { get; set; }

        public string PostLink { get; set; }

        public Uri Thumbnail { get; set; }

        public string Selftext { get; set; }

        public Uri Url { get; set; }

        public SubReddit SubReddit { get; set; }

        public int SubRedditId { get; set; }

        public PostType PostType { get; set; }


        #endregion
    }

    public enum PostType
    {
        Post,
        Image,
        Gif,
        Video
    }
}
