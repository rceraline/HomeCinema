using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HomeCinema.Entities;

namespace HomeCinema.Data.Repositories
{
    public interface IEntityRepository<TEntity> where TEntity : class, IEntityBase, new()
    {
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TEntity> GetSingleAsync(int id);
        void Add(TEntity entity);
        void Edit(TEntity entity);
        void Delete(TEntity entity);
    }
}