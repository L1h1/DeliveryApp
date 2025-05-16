using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
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

        [HttpGet("{email}/email-confirmation/{token}")]
        public async Task<IActionResult> GenerateEmailConfirmationToken([FromRoute] string email, [FromRoute] string token, CancellationToken cancellationToken)
        {
            var result = await _accountService.ConfirmEmailASync(email, token, cancellationToken);

            return Ok(result);
        }

        [HttpPost("{email}/resend-confirmation")]
        public async Task<IActionResult> ResendEmailConfirmation([FromRoute] string email, CancellationToken cancellationToken)
        {
            await _accountService.GenerateEmailConfirmationTokenAsync(email, cancellationToken);

            return Ok(new { Message = "Confirmation email sent." });
        }

        [HttpPost("{email}/reset-password")]
        public async Task<IActionResult> GeneratePasswordResetCode([FromRoute] string email, CancellationToken cancellationToken)
        {
            await _accountService.GeneratePasswordResetTokenAsync(email, cancellationToken);

            return Ok(new { Message = "Code sent." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO resetDTO, CancellationToken cancellationToken)
        {
            var result = await _accountService.ResetPasswordAsync(
                resetDTO.Email, resetDTO.ResetCode, resetDTO.NewPassword, cancellationToken);

            return Ok(result);
        }

        [HttpGet("users/roles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ListRoles(CancellationToken cancellationToken)
        {
            var result = await _accountService.ListRolesAsync(cancellationToken);

            return Ok(result);
        }

        [HttpGet("users/{role}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ListUsersByRole([FromRoute] string role, CancellationToken cancellationToken)
        {
            var result = await _accountService.ListUsersByRoleAsync(role, cancellationToken);

            return Ok(result);
        }
    }
}
