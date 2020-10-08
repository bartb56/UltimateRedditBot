using System;
using System.ComponentModel.DataAnnotations;

namespace UltimateRedditBot.Domain.Common
{
    [Serializable]
    public abstract class BaseEntity<TKey> : IBaseEntity<TKey>
    {
        [Key]
        public virtual TKey Id { get; set; }
    }

    [Serializable]
    public abstract class BaseEntity : BaseEntity<int>, IBaseEntity
    {

    }
}
