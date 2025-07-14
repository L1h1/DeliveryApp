using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserService.BLL.Interfaces;
using UserService.DAL.Interfaces.Repositories;
using UserService.DAL.Models;
using UserService.DAL.Options;

namespace UserService.BLL.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IOptions<JwtOptions> jwtOptions, IUserRepository userRepository, ILogger<TokenService> logger)
        {
            _jwtOptions = jwtOptions.Value;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<string> GenerateAccessTokenAsync(User user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Generating access token for user: @{email}", user.Email);

            var issuer = _jwtOptions.Issuer;
            var audience = _jwtOptions.Audience;
            var key = _jwtOptions.Key;
            var accessTokenExpiration = _jwtOptions.AccessTokenExpirationMins;
            var tokenExpirityTimeStamp = DateTime.Now.AddMinutes(accessTokenExpiration);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha512Signature);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
            };

            var userRoles = await _userRepository.ListUserRolesAsync(user, cancellationToken);

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = tokenExpirityTimeStamp,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = signingCredentials,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);

            _logger.LogInformation("Successfully generated access token for user: @{email}", user.Email);

            return accessToken;
        }

        public (string refreshToken, DateTime expirityDate) GenerateRefreshToken()
        {
            _logger.LogInformation("Generating refresh token");

            var tokenLifetime = _jwtOptions.RefreshTokenExpirationDays;
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            var expirityDate = DateTime.UtcNow.AddDays(tokenLifetime);

            _logger.LogInformation("Successfully generated refresh token");

            return (refreshToken, expirityDate);
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            _logger.LogInformation("Attempting to validate and extract data from access token");

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtOptions.Audience,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key)),
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

            _logger.LogInformation("Successfully extracted data from access token");

            return principal;
        }
    }
}
