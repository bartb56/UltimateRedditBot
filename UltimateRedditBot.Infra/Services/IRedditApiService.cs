using System;
using System.Threading.Tasks;
using UltimateRedditBot.Domain.Enums;
using UltimateRedditBot.Domain.Models;

namespace UltimateRedditBot.Infra.Services
{
    public interface IRedditApiService
    {
        Task<SubReddit> GetSubRedditByName(string name);

        Task<Post> GetNewPost(string subRedditName, Sort sort);

        Task<PostDto> GetOldPost(string subRedditName, string previousName, Sort sort, PostType postType, Guid id);
    }
}
