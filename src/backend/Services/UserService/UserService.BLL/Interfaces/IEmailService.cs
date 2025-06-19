using Microsoft.AspNetCore.Identity.UI.Services;

namespace UserService.BLL.Interfaces
{
    public interface IEmailService : IEmailSender
    {
        Task SendConfirmationEmailAsync(string email, string token, CancellationToken cancellationToken = default);
        Task SendChangeEmailTokenAsync(string userId, string email, string token, CancellationToken cancellationToken = default);
        Task SendResetPasswordEmailAsync(string email, string token, CancellationToken cancellationToken = default);
    }
}
