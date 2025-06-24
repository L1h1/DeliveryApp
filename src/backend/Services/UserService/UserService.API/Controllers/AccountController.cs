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

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("email/confirmation/{email}/{token}")]
        public async Task<IActionResult> GenerateEmailConfirmationToken([FromRoute] string email, [FromRoute] string token, CancellationToken cancellationToken)
        {
            var result = await _accountService.ConfirmEmailASync(email, token, cancellationToken);

            return Ok(result);
        }

        [HttpPost("email/confirmation/resend/{email}")]
        public async Task<IActionResult> ResendEmailConfirmation([FromRoute] string email, CancellationToken cancellationToken)
        {
            await _accountService.GenerateEmailConfirmationTokenAsync(email, cancellationToken);

            return Ok(new { Message = "Confirmation email sent." });
        }

        [HttpPost("password/reset/{email}")]
        public async Task<IActionResult> GeneratePasswordResetCode([FromRoute] string email, CancellationToken cancellationToken)
        {
            await _accountService.GeneratePasswordResetTokenAsync(email, cancellationToken);

            return Ok(new { Message = "Code sent." });
        }

        [HttpPost("password/reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO resetDTO, CancellationToken cancellationToken)
        {
            var result = await _accountService.ResetPasswordAsync(
                resetDTO.Email, resetDTO.ResetCode, resetDTO.NewPassword, cancellationToken);

            return Ok(result);
        }

        [HttpGet("users/roles")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> ListRoles(CancellationToken cancellationToken)
        {
            var result = await _accountService.ListRolesAsync(cancellationToken);

            return Ok(result);
        }

        [HttpGet("users/{role}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> ListUsersByRole([FromRoute] string role, CancellationToken cancellationToken)
        {
            var result = await _accountService.ListUsersByRoleAsync(role, cancellationToken);

            return Ok(result);
        }

        [HttpPost("email/change/{email}")]
        [Authorize]
        public async Task<IActionResult> RequestEmailChange([FromRoute] string email, CancellationToken cancellationToken)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            await _accountService.GenerateEmailChangeTokenAsync(userId, email, cancellationToken);

            return Ok(new { Message = "Confirmation email sent to the new address." });
        }

        [HttpGet("email/change/confirmation/{userId}/{email}/{token}")]
        public async Task<IActionResult> ConfirmEmailChange([FromRoute] string userId, [FromRoute] string email, string token, CancellationToken cancellationToken)
        {
            var result = await _accountService.ConfirmEmailChangeAsync(userId, email, token, cancellationToken);

            return Ok(result);
        }

        [HttpPost("password/change")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO passwordDTO, CancellationToken cancellationToken)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var result = await _accountService.ChangePasswordAsync(userId, passwordDTO.CurrentPassword, passwordDTO.NewPassword, cancellationToken);

            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAuthenticatedUserAccount(CancellationToken cancellationToken)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var result = await _accountService.GetUserByIdAsync(userId, cancellationToken);

            return Ok(result);
        }
    }
}
