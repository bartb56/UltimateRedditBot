namespace UltimateRedditBot.Domain.Common
{
    public interface IBaseEntity<TKey>
    {
        TKey Id { get; }
    }

    public interface IBaseEntity : IBaseEntity<int>
    {

    }
}
