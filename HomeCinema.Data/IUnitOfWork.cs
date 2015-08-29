using System.Threading.Tasks;

namespace HomeCinema.Data
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
    }
}