using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace UltimateRedditBot.Infra.Repository
{
    public interface IRepository<Entity> : IRepository<Entity, int>
    {

    }
    public interface IRepository<Entity, in Key>
    {

        Task<IEnumerable<Entity>> Get(
        Expression<Func<Entity, bool>> filter = null,
        Func<IQueryable<Entity>, IOrderedQueryable<Entity>> orderBy = null,
        string includeProperties = "");
        Task<IEnumerable<Entity>> GetAll();

        Task Insert(Entity entity);

        Task Insert(IEnumerable<Entity> entity);

        Task Update(Entity entity);

        Task Delete(Entity entity);

        Task Delete(Key id);

        IQueryable<Entity> Queriable();

        Task<Entity> GetById(Key id);

        Task<int> Count();
    }
}
