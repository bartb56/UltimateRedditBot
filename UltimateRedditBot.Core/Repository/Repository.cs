using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UltimateRedditBot.Database;
using UltimateRedditBot.Domain.Common;
using UltimateRedditBot.Infra.Repository;

namespace UltimateRedditBot.Core.Repository
{
    public class Repository
    {

    }

    public class Repository<TEntity> : Repository<TEntity, int>
        where TEntity : class, IBaseEntity<int>
    {
        protected Repository(Context context, ILogger<Repository> logger)
            : base(context, logger)
        {
        }
    }

    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : class, IBaseEntity<TKey>
    {
        private readonly Context _context;
        private readonly DbSet<TEntity> _dbSet;
        private readonly ILogger _logger;

        protected Repository(Context context, ILogger<Repository> logger)
        {
            _context = context;
            _logger = logger;
            _dbSet = context.Set<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (includeProperties != null)
                query = includeProperties.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            if (orderBy != null)
                return orderBy(query).ToList();

            return await query.ToListAsync();
        }

        public virtual async Task<TEntity> GetById(TKey id)
        {
            var entity = await _dbSet.FindAsync(id);
            return entity;
        }

        public async Task Insert(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await SaveChanges();
        }

        public virtual async Task Insert(IEnumerable<TEntity> entity)
        {
            await _dbSet.AddRangeAsync(entity);
            await SaveChanges();
        }

        public virtual async Task Delete(TKey id)
        {
            var entity = await _dbSet.FindAsync(id);
            await Delete(entity);
            await SaveChanges();
        }

        public async Task Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
            await SaveChanges();
        }

        public async Task Delete(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
            await SaveChanges();
        }

        public async Task Update(TEntity entity)
        {
            _dbSet.Update(entity);
            await SaveChanges();
        }

        public async Task UpdateRange(IEnumerable<TEntity> entities)
        {
            _dbSet.UpdateRange(entities);
            await SaveChanges();
        }

        public virtual async Task<IAsyncEnumerable<TEntity>> GetAll()
        {
            IQueryable<TEntity> query = _dbSet;
            return query.ToAsyncEnumerable();
        }

        protected IQueryable<TEntity> Queryable()
        {
            return _dbSet.AsQueryable();
        }

        private async Task SaveChanges()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Critical, e, "Error saving changes");
            }
        }

        public int Count()
            => _dbSet.Count();
    }
}
