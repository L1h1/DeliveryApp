using Microsoft.AspNetCore.Identity;
using UserService.BLL.DTOs.Response;
using UserService.DAL.Models;

namespace UserService.BLL.Interfaces
{
    public interface IAccountService
    {
        Task GenerateEmailConfirmationTokenAsync(string email, CancellationToken cancellationToken = default);
        Task<IdentityResult> ConfirmEmailASync(string email, string token, CancellationToken cancellationToken = default);
        Task GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default);
        Task<IdentityResult> ResetPasswordAsync(string email, string resetCode, string newPassword, CancellationToken cancellationToken = default);
        Task<List<string>> ListRolesAsync(CancellationToken cancellationToken = default);
        Task<List<UserDetailsDTO>> ListUsersByRoleAsync(string role, CancellationToken cancellationToken = default);
    }
}
