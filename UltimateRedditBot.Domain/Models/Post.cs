using System;
using UltimateRedditBot.Domain.Common;

namespace UltimateRedditBot.Domain.Models
{
    public class Post : BaseEntity
    {
        #region Constructor

        public Post()
        {
            PostType = PostType.Post;

            if (Url != null)
            {
                PostType = PostType.Image;

                var url = Url.ToString();

                if (url.Contains(".gif") || url.Contains("https://gfycat") || url.Contains("https://redgifs"))
                    PostType = PostType.Gif;

                if (url.Contains(".mp4"))
                    PostType = PostType.Image;
            }
        }


        public Post(string postId, string author, int downs, int ups, bool isOver18, string title, string postLink, Uri thumbnail, string selfText, Uri url, PostType postType)
        {
            PostId = postId;
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
            if (Url != null)
            {
                var url = Url.ToString();

                if (url.Contains(".gif") || url.Contains("https://gfycat") || url.Contains("https://redgifs"))
                    PostType = PostType.Gif;

                if (url.Contains(".jpg") || url.Contains(".png") || url.Contains(".jpeg"))
                    PostType = PostType.Gif;

                if (url.Contains(".mp4"))
                    PostType = PostType.Video;
            }

            return PostType;
        }

        public void UpdateUpsAndDowns(int downs, int ups)
        {
            Downs = downs;
            Ups = ups;
        }

        #endregion

        #region Properties

        public string PostId { get; set; }

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
