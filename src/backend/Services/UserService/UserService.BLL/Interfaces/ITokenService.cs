using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserService.DAL.Models;

namespace UserService.BLL.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateAccessTokenAsync(User user, CancellationToken cancellationToken = default);
        (string refreshToken, DateTime expirityDate) GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromToken(string token);
    }
}
