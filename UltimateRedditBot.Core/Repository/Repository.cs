using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UltimateRedditBot.Database;
using UltimateRedditBot.Domain.Common;
using UltimateRedditBot.Infra.Repository;

namespace UltimateRedditBot.Core.Repository
{
    public class Repository<Entity> : Repository<Entity, int>, IRepository<Entity, int>
        where Entity : class, IBaseEntity<int>
    {
        public Repository(Context context)
            : base(context)
        {
        }
    }

    public class Repository<Entity, Key> : IRepository<Entity, Key>
        where Entity : class, IBaseEntity<Key>
    {
        internal Context context;
        internal DbSet<Entity> dbSet;

        public Repository(Context context)
        {
           this.context = context;
           dbSet = context.Set<Entity>();
        }

        public virtual async Task<IEnumerable<Entity>> Get(
        Expression<Func<Entity, bool>> filter = null,
        Func<IQueryable<Entity>, IOrderedQueryable<Entity>> orderBy = null,
        string includeProperties = "")
        {
            IQueryable<Entity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }


            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        public async virtual Task<Entity> GetById(Key id)
        {
            return await dbSet.FindAsync(id);
        }

        public async virtual Task Insert(Entity entity)
        {
            await dbSet.AddAsync(entity);
        }

        public async Task Insert(IEnumerable<Entity> entity)
        {
            await dbSet.AddRangeAsync(entity);
        }

        public async virtual Task Delete(Key id)
        {
            var entity = await GetById(id);
            await Delete(entity); 
        }

        public async virtual Task Delete(Entity entity)
        {
            if (context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
        }

        public async virtual Task Update(Entity entity)
        {
            dbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
        }

        public async virtual Task<IEnumerable<Entity>> GetAll()
        {
            IQueryable<Entity> query = dbSet;

            return await query.ToListAsync();
        }

        public virtual void SaveChanges()
        {
            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public IQueryable<Entity> Queriable()
        {
            return dbSet.AsQueryable();
        }

        public async Task<int> Count()
            => dbSet.Count();
    }
}
