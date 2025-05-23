using Microsoft.AspNetCore.Identity;
using UserService.BLL.DTOs.Response;

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
        Task GenerateEmailChangeTokenAsync(string userId, string email, CancellationToken cancellationToken = default);
        Task<IdentityResult> ConfirmEmailChangeAsync(string userId, string email, string token, CancellationToken cancellationToken = default);
        Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
        Task<UserDetailsDTO> GetUserByIdAsync(string userId,  CancellationToken cancellationToken = default);
    }
}
