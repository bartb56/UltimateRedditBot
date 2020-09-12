namespace UltimateRedditBot.Domain.Common
{
    public interface IBaseEntity<Key>
    {
        Key Id { get; }
    }

    public interface IBaseEntity : IBaseEntity<int>
    {

    }
}
