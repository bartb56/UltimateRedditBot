using System;
using System.ComponentModel.DataAnnotations;

namespace UltimateRedditBot.Domain.Common
{
    [Serializable]
    public abstract class BaseEntity<Key> : IBaseEntity<Key>
    {
        [Key]
        public virtual Key Id { get; set; }
    }

    [Serializable]
    public abstract class BaseEntity : BaseEntity<int>, IBaseEntity
    {

    }
}
