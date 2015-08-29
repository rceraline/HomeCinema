using System.Collections.Generic;
using System.Threading.Tasks;
using HomeCinema.Entities;

namespace HomeCinema.Services
{
    public interface IMembershipService
    {
        Task<User> CreateUser(string username, string email, string password, int[] roles);
        Task<User> GetUserAsync(int userId);
        Task<List<Role>> GetUserRolesAsync(string username);
        Task<MembershipContext> ValidateUserAsync(string username, string password);
    }
}