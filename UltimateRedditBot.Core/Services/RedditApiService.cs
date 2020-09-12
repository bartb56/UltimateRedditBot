using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UltimateRedditBot.Core.Constants;
using UltimateRedditBot.Domain.Api;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Domain.Queue;
using UltimateRedditBot.Infra.Services;

namespace UltimateRedditBot.Core.Services
{
    public class RedditApiService : IRedditApiService
    {
        #region Fields

        private readonly HttpClient _client = new HttpClient();

        #endregion

        #region Constructor

        #endregion

        #region Methods

        public async Task<SubReddit> GetSubRedditByName(string name)
        {
            var request = await _client.GetAsync(string.Format(RedditApiConstants.SearchSubRedditByNameUrl, name));

            if (!request.IsSuccessStatusCode)
                return null;

            var responseBody = await request.Content.ReadAsStringAsync();
            return ParseSubReddit(name, responseBody);
        }
        
        public async Task<PostDto> GetPost(QueueItem queueItem, string lastUsedName, string subredditName)
        {
            for (int i = 0; i < RedditApiConstants.MaximumAttempts; i++)
            {

                var url = string.Format(RedditApiConstants.GetRedditPostBase, subredditName, lastUsedName);
                var request = await _client.GetAsync(url);

                if (!request.IsSuccessStatusCode)
                    return null;

                var responseBody = await request.Content.ReadAsStringAsync();


                var post = GetPostFromApi(responseBody, queueItem.SubRedditId, queueItem.PostType, out string newLastName);
                if (post != null)
                {
                    return new PostDto { Post = post, QueueItemId = queueItem.Id };
                }

                if (string.IsNullOrEmpty(newLastName))
                    return null; //no new posts found.

                lastUsedName = newLastName;
            }
             
            return null;
            
        }

        #region Utils


        private SubReddit ParseSubReddit(string name, string responseData)
        {
            dynamic data = JObject.Parse(responseData);

            try
            {
                var baseElement = data.data.children[0].data;
                var displayName = (string)baseElement.display_name;

                if (!ValidSubReddit(name, displayName))
                    return null;

                var isOver18 = (bool)baseElement.over18;
                return new SubReddit(name, isOver18);

            }
            catch(Exception e)
            {

            }

            return null;

        }

        private static bool ValidSubReddit(string name, string responseName)
            => name.ToUpper() == responseName.ToUpper();


        private Post GetPostFromApi(string responseBody, int subRedditId, PostType postType, out string newLastUsedName)
        {
            newLastUsedName = string.Empty;

            var posts = ParseData(responseBody, subRedditId);

            if (posts == null || !posts.Any())
                return null;

            Post post = null;

            if (postType == PostType.Image)
                post = posts.FirstOrDefault(post => post.GetPostType() == PostType.Gif || post.GetPostType() == PostType.Image || post.GetPostType() == PostType.Video);

            if (postType == PostType.Gif)
                post = posts.FirstOrDefault(post => post.GetPostType() == PostType.Gif);

            if (postType == PostType.Video)
                post = posts.FirstOrDefault(post => post.GetPostType() == PostType.Video);

            if (postType == PostType.Post)
                post = posts.FirstOrDefault(post => post.GetPostType() == PostType.Post);

            newLastUsedName = posts.Last().PostId;
            return post;
        }

        private IEnumerable<Post> ParseData(string response, int subRedditId)
        {
            var apiPosts = JsonConvert.DeserializeObject<ApiSubReddit>(response);
            if (apiPosts == null || apiPosts.Data == null || apiPosts.Data.Children == null)
                return null;

            var posts = apiPosts.Data.Children.Select(x => x.Data)
                .Select(x => 
                    new Post { Author = x.Author, PostId = x.Name, IsOver18 = x.Over18, Selftext = x.Selftext, Thumbnail = x.Thumbnail, Title = x.Title, PostLink = x.Permalink, Url = x.Url, SubRedditId = subRedditId });

            return posts;
        }
        

        #endregion

        #endregion
    }
}
