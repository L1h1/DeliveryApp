using Microsoft.AspNetCore.Identity;
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

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Regiser([FromBody] RegisterRequestDTO userDTO, CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterUserAsync(userDTO, cancellationToken);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO userDTO, CancellationToken cancellationToken)
        {
            var result = await _authService.LoginUserAsync(userDTO, cancellationToken);

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> UpdateTokens([FromBody] TokenRequestDTO tokenDTO, CancellationToken cancellationToken)
        {
            var result = await _authService.UpdateTokensAsync(tokenDTO, cancellationToken);

            return Ok(result);
        }
    }
}
