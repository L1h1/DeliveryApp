using Microsoft.AspNetCore.Mvc;
using UserService.BLL.DTOs.Request;
using UserService.BLL.Interfaces;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IAccountService _accountService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, IAccountService accountService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _accountService = accountService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Regiser([FromBody] RegisterRequestDTO userDTO, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to register new user: @{email}", userDTO.Email);

            var result = await _authService.RegisterUserAsync(userDTO, cancellationToken);

            await _accountService.GenerateEmailConfirmationTokenAsync(userDTO.Email, cancellationToken);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO userDTO, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to authenticate user: @{email}", userDTO.Email);

            var result = await _authService.LoginUserAsync(userDTO, cancellationToken);

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> UpdateTokens([FromBody] TokenRequestDTO tokenDTO, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to refresh access token");

            var result = await _authService.UpdateTokensAsync(tokenDTO, cancellationToken);

            return Ok(result);
        }
    }
}
