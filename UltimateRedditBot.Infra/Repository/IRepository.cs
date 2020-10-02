using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace UltimateRedditBot.Infra.Repository
{
    public interface IRepository<TEntity> : IRepository<TEntity, int>
    {

    }
    public interface IRepository<TEntity, in TKey>
    {

        Task<IEnumerable<TEntity>> Get(
        Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "");
        Task<IAsyncEnumerable<TEntity>> GetAll();

        Task Insert(TEntity entity);

        Task Insert(IEnumerable<TEntity> entity);

        Task Update(TEntity entity);

        Task UpdateRange(IEnumerable<TEntity> entities);

        Task Delete(TEntity entity);

        Task Delete(IEnumerable<TEntity> entities);

        Task Delete(TKey id);

        Task<TEntity> GetById(TKey id);

        int Count();
    }
}
