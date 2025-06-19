using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.DAL.Interfaces.Caching;
using UserService.DAL.Interfaces.Repositories;
using UserService.DAL.Models;

namespace UserService.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ICacheService _cacheService;

        public UserRepository(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager, ICacheService cacheService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _cacheService = cacheService;
        }

        public async Task AddRoleAsync(string name, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _roleManager.CreateAsync(new IdentityRole<Guid>(name));
        }

        public async Task<IdentityResult> AddUserAsync(User user, string password, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _userManager.CreateAsync(user, password);

            await _cacheService.SetAsync($"{typeof(User).Name}:{user.Id}", user, cancellationToken);

            return result;
        }

        public async Task<IdentityResult> AssignRoleAsync(User user, string role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _userManager.AddToRoleAsync(user, role);

            await _cacheService.SetAsync($"{typeof(User).Name}:{user.Id}", user, cancellationToken);

            return result;
        }

        public async Task<IdentityResult> ConfirmEmailASync(User user, string token, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _userManager.ConfirmEmailAsync(user, token);

            await _cacheService.SetAsync($"{typeof(User).Name}:{user.Id}", user, cancellationToken);

            return result;
        }

        public async Task<IdentityResult> ConfirmEmailChangeAsync(User user, string newEmail, string token, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _userManager.ChangeEmailAsync(user, newEmail, token);

            await _cacheService.SetAsync($"{typeof(User).Name}:{user.Id}", user, cancellationToken);

            return result;
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            await _cacheService.SetAsync($"{typeof(User).Name}:{user.Id}", user, cancellationToken);

            return result;
        }

        public async Task<string> GenerateEmailChangeTokenAsync(User user, string newEmail, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);

            return result;
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            return result;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _userManager.GeneratePasswordResetTokenAsync(user);

            return result;
        }

        public async Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var key = $"{typeof(User).Name}:{email}";

            var cached = await _cacheService.GetAsync<User>(key, cancellationToken);

            if (cached is not null)
            {
                return cached;
            }

            var user = await _userManager.FindByEmailAsync(email);

            await _cacheService.SetAsync(key, user, cancellationToken);

            return user;
        }

        public async Task<User> GetUserByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var key = $"{typeof(User).Name}:{id}";

            var cached = await _cacheService.GetAsync<User>(key, cancellationToken);

            if (cached is not null)
            {
                return cached;
            }

            var user = await _userManager.FindByIdAsync(id);

            await _cacheService.SetAsync(key, user, cancellationToken);

            return user;
        }

        public async Task<List<string>> ListRolesAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var key = "Roles";
            var cached = await _cacheService.GetAsync<List<string?>>(key, cancellationToken);

            if (cached is not null)
            {
                return cached;
            }

            var result = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

            await _cacheService.SetAsync(key, result, cancellationToken);

            return result;
        }

        public async Task<List<string>> ListUserRolesAsync(User user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var key = $"Roles:{typeof(User).Name}:{user.Id}";
            var cached = await _cacheService.GetAsync<List<string?>>(key, cancellationToken);

            if (cached is not null)
            {
                return cached;
            }

            var result = (await _userManager.GetRolesAsync(user)).ToList();

            await _cacheService.SetAsync(key, result, cancellationToken);

            return result;
        }

        public async Task<List<User>> ListUsersAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _userManager.Users.ToListAsync();

            return result;
        }

        public async Task<List<User>> ListUsersByRoleAsync(string role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = (await _userManager.GetUsersInRoleAsync(role)).ToList();

            return result;
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string resetCode, string newPassword, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _userManager.ResetPasswordAsync(user, resetCode, newPassword);

            await _cacheService.SetAsync($"{typeof(User).Name}:{user.Id}", user, cancellationToken);

            return result;
        }

        public async Task<bool> RoleExistsAsync(string name, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var key = "Roles";

            var cached = await _cacheService.GetAsync<List<string?>>(key, cancellationToken);

            if (cached is not null)
            {
                return cached.Contains(name);
            }

            var result = await _roleManager.RoleExistsAsync(name);

            await _cacheService.SetAsync(key, await ListRolesAsync(cancellationToken), cancellationToken);

            return result;
        }

        public async Task<IdentityResult> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _userManager.UpdateAsync(user);

            await _cacheService.SetAsync($"{typeof(User).Name}:{user.Id}", user, cancellationToken);

            return result;
        }
    }
}
