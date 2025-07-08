using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.DAL.Models;
using UserService.DAL.Options;

namespace UserService.Tests
{
    public class TestDataProvider
    {
        public static User SampleTestUser => new()
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            UserName = "testuser",
        };

        public static TokenValidationParameters TestTokenValidationParameters(JwtOptions jwtOptions) => new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
        };
    }
}
