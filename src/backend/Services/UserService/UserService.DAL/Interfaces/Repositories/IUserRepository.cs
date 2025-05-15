using Microsoft.AspNetCore.Identity;
using UserService.DAL.Models;

namespace UserService.DAL.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> ListUsersAsync(CancellationToken cancellationToken = default);
        Task<User> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<IdentityResult> AddUserAsync(User user, CancellationToken cancellationToken = default);
        Task<IdentityResult> UpdateUserAsync(User user, CancellationToken cancellationToken = default);
        Task<string> GenerateEmailConfirmationTokenAsync(User user, CancellationToken cancellationToken = default);
        Task<IdentityResult> ConfirmEmailASync(User user, string token, CancellationToken cancellationToken = default);
        Task<string> GeneratePasswordResetTokenAsync(User user, CancellationToken cancellationToken = default);
        Task<IdentityResult> ResetPasswordAsync(User user, string resetCode, string newPassword, CancellationToken cancellationToken = default);
        Task<List<string>> ListRolesAsync(CancellationToken cancellationToken = default);
        Task AddRoleAsync(string name, CancellationToken cancellationToken = default);
        Task<bool> RoleExistsAsync(string name, CancellationToken cancellationToken = default);
        Task<List<User>> ListUsersByRoleAsync(string role, CancellationToken cancellationToken = default);
    }
}
