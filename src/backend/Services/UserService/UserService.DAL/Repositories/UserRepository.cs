using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.DAL.Interfaces.Repositories;
using UserService.DAL.Models;

namespace UserService.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public UserRepository(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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

            return result;
        }

        public async Task<IdentityResult> AssignRoleAsync(User user, string role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _userManager.AddToRoleAsync(user, role);

            return result;
        }

        public async Task<IdentityResult> ConfirmEmailASync(User user, string token, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _userManager.ConfirmEmailAsync(user, token);

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

            var user = await _userManager.FindByEmailAsync(email);

            return user;
        }

        public async Task<User> GetUserByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userManager.FindByIdAsync(id);

            return user;
        }

        public async Task<List<string>> ListRolesAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

            return result;
        }

        public async Task<List<string>> ListUserRolesAsync(User user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = (await _userManager.GetRolesAsync(user)).ToList();

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

            return result;
        }

        public async Task<bool> RoleExistsAsync(string name, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await RoleExistsAsync(name, cancellationToken);

            return result;
        }

        public async Task<IdentityResult> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _userManager.UpdateAsync(user);

            return result;
        }
    }
}
