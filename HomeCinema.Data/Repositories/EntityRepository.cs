using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HomeCinema.Entities;

namespace HomeCinema.Data.Repositories
{
    public class EntityRepository<TEntity> : IEntityRepository<TEntity> where TEntity : class, IEntityBase, new()
    {
        private HomeCinemaContext _dbContext;

        protected IDbFactory DbFactory { get; private set; }

        protected HomeCinemaContext DbContext
        {
            get { return _dbContext ?? (_dbContext = DbFactory.Init()); }
        }

        public EntityRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return DbContext.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = DbContext.Set<TEntity>();
            return includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        public async Task<TEntity> GetSingleAsync(int id)
        {
            var entity = await GetAll()
                .SingleOrDefaultAsync(e => e.Id == id)
                .ConfigureAwait(false);
            return entity;
        }

        public virtual void Add(TEntity entity)
        {
            DbContext.Entry(entity);
            DbContext.Set<TEntity>().Add(entity);
        }

        public virtual void Edit(TEntity entity)
        {
            var dbEntityEntry = DbContext.Entry(entity);
            dbEntityEntry.State = EntityState.Modified;
        }

        public virtual void Delete(TEntity entity)
        {
            var dbEntityEntry = DbContext.Entry(entity);
            dbEntityEntry.State = EntityState.Deleted;
        }
    }
}
