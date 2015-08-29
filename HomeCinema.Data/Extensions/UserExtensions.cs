using System.Data.Entity;
using System.Threading.Tasks;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;

namespace HomeCinema.Data.Extensions
{
    public static class UserExtensions
    {
        public static async Task<User> GetByUserNameAsync(this IEntityRepository<User> userRepository, string username)
        {
            var user = await userRepository.GetAll()
                .FirstOrDefaultAsync(u => u.Username == username)
                .ConfigureAwait(false);
            return user;
        }
    }
}
