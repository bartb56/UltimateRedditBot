namespace UltimateRedditBot.Core.Constants
{
    public static class RedditApiConstants
    {
        public const string SearchSubRedditByNameUrl = "https://www.reddit.com/subreddits/search.json?q={0}&include_over_18=on&limit=1";

        public const string GetRedditPostBase = "https://reddit.com/r/{0}/new/.json?limit=10&after={1}";

        public const int MaximumAttempts = 1000;
    }
}
