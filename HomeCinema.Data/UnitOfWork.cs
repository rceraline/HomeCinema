using System.Threading.Tasks;

namespace HomeCinema.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbFactory _dbFactory;
        private HomeCinemaContext _dbContext;

        public HomeCinemaContext DbContext
        {
            get { return _dbContext ?? (_dbContext = _dbFactory.Init()); }
        }

        public UnitOfWork(IDbFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public Task CommitAsync()
        {
            return DbContext.SaveChangesAsync();
        }
    }
}
