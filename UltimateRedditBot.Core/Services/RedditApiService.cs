using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UltimateRedditBot.Core.Constants;
using UltimateRedditBot.Domain.Api;
using UltimateRedditBot.Domain.Enums;
using UltimateRedditBot.Domain.Models;
using UltimateRedditBot.Infra.Services;

namespace UltimateRedditBot.Core.Services
{
    public class RedditApiService : IRedditApiService
    {
        #region Fields

        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<RedditApiService> _logger;
        //private readonly ILogger _logger;

        #endregion

        #region Constructor

        public RedditApiService(IHttpClientFactory clientFactory, ILogger<RedditApiService> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            //_logger = logger;
        }

        #endregion

        #region Methods

        public async Task<SubReddit> GetSubRedditByName(string name)
        {
            var client = _clientFactory.CreateClient();
            var request = await client.GetAsync(string.Format(RedditApiConstants.SearchSubRedditByNameUrl, name));

            if (!request.IsSuccessStatusCode)
            {
                _logger.LogError($"Getting a subreddit by name failed. name: {name}, error: {request.StatusCode}");
                return null;
            }

            var responseBody = await request.Content.ReadAsStringAsync();
            return ParseSubReddit(name, responseBody);
        }

        public async Task<Post> GetNewPost(string subRedditName, Sort sort)
        {
            return await Process(1, subRedditName, "before", "", sort);
        }

        public async Task<PostDto> GetOldPost(string subRedditName, string previousName, Sort sort, PostType postType, Guid id)
        {
            var post = await Process(RedditApiConstants.MaximumAttempts, subRedditName, "after", previousName, sort, postType);
            if (post is null)
                return null;

            return new PostDto
            {
                Post = post,
                QueueItemId = id
            };
        }

        private async Task<Post> Process(int maximumAttempts, string subRedditName, string beforeOrAfter, string previousName, Sort sort, PostType postType = PostType.Image)
        {
            for (var i = 0; i < maximumAttempts; i++)
            {
                var url = string.Format(RedditApiConstants.GetRedditPostBase, subRedditName, sort.ToString().ToLowerInvariant(), beforeOrAfter, previousName);

                var client = _clientFactory.CreateClient();
                var request = await client.GetAsync(url);

                if (!request.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error getting new post. {url}, {request.StatusCode}");
                    return null;
                }

                var responseBody = await request.Content.ReadAsStringAsync();
                var post = ProccessRequest(responseBody, out previousName, postType);

                if (post is not null)
                    return post;
            }

            //If we got this far we are not able to find any posts.
            return null;
        }

        #region Utils

        private static Post ProccessRequest(string request, out string previousPostName, PostType postType)
        {
            previousPostName = string.Empty;

            var posts = ParseRequest(request).ToList();
            if (!posts.Any())
                return null;

            var post = postType switch
            {
                PostType.Image => posts.FirstOrDefault(x =>
                    x.GetPostType() == PostType.Gif || x.GetPostType() == PostType.Image ||
                    x.GetPostType() == PostType.Video),
                PostType.Gif => posts.FirstOrDefault(x => x.GetPostType() == PostType.Gif),
                PostType.Video => posts.FirstOrDefault(x => x.GetPostType() == PostType.Video),
                PostType.Post => posts.FirstOrDefault(x => x.GetPostType() == PostType.Post),
                _ => null
            };

            previousPostName = posts.Last().Id;
            return post;
        }

        private static IEnumerable<Post> ParseRequest(string request)
        {
            var apiPosts = JsonConvert.DeserializeObject<ApiSubReddit>(request);

            var posts = apiPosts?.Data?.Children?.Select(x => x.Data)
                .Select(x =>
                {
                    var thumbsUp = int.Parse(x.Ups);

                    return new Post
                    {
                        Author = x.Author, Id = x.Name, IsOver18 = x.Over18, Selftext = x.Selftext,
                        Thumbnail = x.Thumbnail, Title = x.Title, PostLink = x.Permalink, Url = x.Url, Ups = thumbsUp
                    };
                });

            return posts;
        }

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

                //throw new Exception("u fked up");
                return new SubReddit(name, isOver18);

            }
            catch(Exception e)
            {
               //_logger.LogError(e, " encountered an error while parsing the subreddit");
            }

            return null;

        }

        private static bool ValidSubReddit(string name, string responseName)
            => name.Equals(responseName, StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion
    }
}
