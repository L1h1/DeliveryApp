using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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
        private readonly IOptions<JwtOptions> _jwtOptions;
        private readonly IUserRepository _userRepository;

        public TokenService(IOptions<JwtOptions> jwtOptions, IUserRepository userRepository)
        {
            _jwtOptions = jwtOptions;
            _userRepository = userRepository;
        }

        public async Task<string> GenerateAccessTokenAsync(User user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var issuer = _jwtOptions.Value.Issuer;
            var audience = _jwtOptions.Value.Audience;
            var key = _jwtOptions.Value.Key;
            var accessTokenExpiration = _jwtOptions.Value.AccessTokenExpirationMins;
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

            return accessToken;
        }

        public (string refreshToken, DateTime expirityDate) GenerateRefreshToken()
        {
            var tokenLifetime = _jwtOptions.Value.RefreshTokenExpirationDays;
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            var expirityDate = DateTime.UtcNow.AddDays(tokenLifetime);

            return (refreshToken, expirityDate);
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Value.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtOptions.Value.Audience,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.Key)),
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
    }
}
