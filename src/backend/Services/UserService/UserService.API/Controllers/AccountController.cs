using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.BLL.DTOs.Request;
using UserService.BLL.Interfaces;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountService accountService, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpGet("email-confirmation/{email}/{token}")]
        public async Task<IActionResult> GenerateEmailConfirmationToken([FromRoute] string email, [FromRoute] string token, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to generate email confirmation for: @{email}", email);

            var result = await _accountService.ConfirmEmailASync(email, token, cancellationToken);

            return Ok(result);
        }

        [HttpPost("resend-confirmation/{email}")]
        public async Task<IActionResult> ResendEmailConfirmation([FromRoute] string email, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to resend email confirmation for: @{email}", email);

            await _accountService.GenerateEmailConfirmationTokenAsync(email, cancellationToken);

            return Ok(new { Message = "Confirmation email sent." });
        }

        [HttpPost("reset-password/{email}")]
        public async Task<IActionResult> GeneratePasswordResetCode([FromRoute] string email, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to generate password reset code for: @{email}", email);

            await _accountService.GeneratePasswordResetTokenAsync(email, cancellationToken);

            return Ok(new { Message = "Code sent." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO resetDTO, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to reset password for: @{email}", resetDTO.Email);

            var result = await _accountService.ResetPasswordAsync(
                resetDTO.Email, resetDTO.ResetCode, resetDTO.NewPassword, cancellationToken);

            return Ok(result);
        }

        [HttpGet("users/roles")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> ListRoles(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to list all roles");

            var result = await _accountService.ListRolesAsync(cancellationToken);

            return Ok(result);
        }

        [HttpGet("users/{role}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> ListUsersByRole([FromRoute] string role, CancellationToken cancellationToken)
        {
            _logger.LogInformation("@{name} is attempting to get all users with role: @{role}", User.Identity.Name, role);

            var result = await _accountService.ListUsersByRoleAsync(role, cancellationToken);

            return Ok(result);
        }

        [HttpPost("change-email/{email}")]
        [Authorize]
        public async Task<IActionResult> RequestEmailChange([FromRoute] string email, CancellationToken cancellationToken)
        {
            _logger.LogInformation("@{User.Identity.Name} is attempting to change email to: @{email}", User.Identity.Name, email);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            await _accountService.GenerateEmailChangeTokenAsync(userId, email, cancellationToken);

            return Ok(new { Message = "Confirmation email sent to the new address." });
        }

        [HttpGet("confirm-email-change/{userId}/{email}/{token}")]
        public async Task<IActionResult> ConfirmEmailChange([FromRoute] string userId, [FromRoute] string email, string token, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to confirm email for: @{email}", email);

            var result = await _accountService.ConfirmEmailChangeAsync(userId, email, token, cancellationToken);

            return Ok(result);
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO passwordDTO, CancellationToken cancellationToken)
        {
            _logger.LogInformation("@{name} is attempting to change password", User.Identity.Name);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var result = await _accountService.ChangePasswordAsync(userId, passwordDTO.CurrentPassword, passwordDTO.NewPassword, cancellationToken);

            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAuthenticatedUserAccount(CancellationToken cancellationToken)
        {
            _logger.LogInformation("@{name} is attempting to retrieve account info", User.Identity.Name);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var result = await _accountService.GetUserByIdAsync(userId, cancellationToken);

            return Ok(result);
        }
    }
}
