using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using HomeCinema.Data;
using HomeCinema.Data.Extensions;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;

namespace HomeCinema.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IEntityRepository<User> _userRepository;
        private readonly IEntityRepository<Role> _roleRepository;
        private readonly IEntityRepository<UserRole> _userRoleRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IUnitOfWork _unitOfWork;

        public MembershipService(
            IEntityRepository<User> userRepository,
            IEntityRepository<Role> roleRepository,
            IEntityRepository<UserRole> userRoleRepository,
            IEncryptionService encryptionService,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _encryptionService = encryptionService;
            _unitOfWork = unitOfWork;
        }

        public async Task<User> CreateUser(string username, string email, string password, int[] roles)
        {
            var existingUser = await _userRepository.GetByUserNameAsync(username).ConfigureAwait(false);
            if (existingUser != null)
            {
                throw new Exception("Username is already in use.");
            }

            var passwordSalt = _encryptionService.CreateSalt();

            var user = await AddUserInDatabaseAsync(username, email, password, passwordSalt).ConfigureAwait(false);
            await AssignRolesAsync(user, roles).ConfigureAwait(false);
            return user;
        }

        public Task<User> GetUserAsync(int userId)
        {
            return _userRepository.GetSingleAsync(userId);
        }

        public async Task<List<Role>> GetUserRolesAsync(string username)
        {
            var existingUser = await _userRepository.GetByUserNameAsync(username).ConfigureAwait(false);
            if (existingUser == null)
            {
                throw new NullReferenceException("User does not exist.");
            }

            var roles = existingUser.UserRoles.Select(ur => ur.Role).Distinct().ToList();
            return roles;
        }

        public async Task<MembershipContext> ValidateUserAsync(string username, string password)
        {
            var user = await _userRepository.GetByUserNameAsync(username).ConfigureAwait(false);
            if (user == null)
            {
                throw new Exception("User does not exist.");
            }

            var membershipContext = new MembershipContext();
            if (!IsUserValid(user, password))
            {
                return membershipContext;
            }

            var userRoles = await GetUserRolesAsync(user.Username).ConfigureAwait(false);
            membershipContext.User = user;

            var userRoleNames = userRoles.Select(ur => ur.Name).ToArray();
            var identity = new GenericIdentity(user.Username);
            membershipContext.Principal = new GenericPrincipal(identity, userRoleNames);

            return membershipContext;
        }

        private async Task<User> AddUserInDatabaseAsync(string username, string email, string password, string passwordSalt)
        {
            var user = new User
            {
                Username = username,
                Salt = passwordSalt,
                Email = email,
                IsLocked = false,
                HashedPassword = _encryptionService.EncryptPassword(password, passwordSalt),
                DateCreated = DateTime.UtcNow
            };

            _userRepository.Add(user);
            await _unitOfWork.CommitAsync().ConfigureAwait(false);
            return user;
        }

        private async Task AssignRolesAsync(User user, int[] roles)
        {
            if (roles != null && roles.Length > 0)
            {
                foreach (var role in roles)
                {
                    await AddUserToRoleAsync(user, role).ConfigureAwait(false);
                }
            }

            await _unitOfWork.CommitAsync().ConfigureAwait(false);
        }

        private async Task AddUserToRoleAsync(User user, int roleId)
        {
            var role = await _roleRepository.GetSingleAsync(roleId).ConfigureAwait(false);
            if (role == null)
            {
                throw new ApplicationException("Role does not exist.");
            }

            var userRole = new UserRole
            {
                RoleId = role.Id,
                UserId = user.Id
            };
            _userRoleRepository.Add(userRole);
        }

        private bool IsPasswordValid(User user, string password)
        {
            var encryptPassword = _encryptionService.EncryptPassword(password, user.Salt);
            var isPasswordValid = string.Equals(encryptPassword, user.HashedPassword);
            return isPasswordValid;
        }

        private bool IsUserValid(User user, string password)
        {
            return IsPasswordValid(user, password) &&
                   !user.IsLocked;
        }
    }
}
