using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UserService.BLL.Services;
using UserService.DAL.Interfaces.Repositories;
using UserService.DAL.Models;
using UserService.DAL.Options;

namespace UserService.Tests.UnitTests.Services
{
    public class TokenServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IOptions<JwtOptions>> _jwtOptionsMock;
        private readonly TokenService _service;

        private readonly JwtOptions _testJwtOptions = new JwtOptions
        {
            Issuer = "test_issuer",
            Audience = "test_audience",
            Key = "SOMEBODYONCETOLDMETHEWORLDISGONNAROLLMESOMEBODYONCETOLDMETHEWORLD",
            AccessTokenExpirationMins = 60,
            RefreshTokenExpirationDays = 7
        };

        public TokenServiceTests()
        {
            _userRepositoryMock = new();
            _jwtOptionsMock = new();
            _jwtOptionsMock.Setup(m => m.Value).Returns(_testJwtOptions);

            _service = new(_jwtOptionsMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task GenerateAccessTokenAsync_ShouldReturnValidToken()
        {
            //Arrange
            var token = CancellationToken.None;
            var testUser = TestDataProvider.SampleTestUser;
            var userRoles = new List<string> { "Admin" };
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = TestDataProvider.TestTokenValidationParameters(_testJwtOptions);

            _userRepositoryMock.Setup(m => m.ListUserRolesAsync(testUser, token)).ReturnsAsync(userRoles);

            //Act
            var result = await _service.GenerateAccessTokenAsync(testUser, token);

            //Assert
            result.Should().NotBeNullOrEmpty();
            
            var principal = tokenHandler.ValidateToken(result, validationParameters, out var validatedToken);
            var jwtSecurityToken = validatedToken as JwtSecurityToken;
            
            jwtSecurityToken.Header.Alg.Should().Be(SecurityAlgorithms.HmacSha512);
            principal.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == testUser.Id.ToString());
            principal.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == testUser.Email);
            principal.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == testUser.UserName);
            principal.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
            _userRepositoryMock.Verify(m => m.ListUserRolesAsync(testUser, token), Times.Once);
        }

        [Fact]
        public async Task GetPrincipalFromToken_ShouldReturnValidPrincipal_ForCorrectToken()
        {
            // Arrange
            var token = CancellationToken.None;
            var testUser = TestDataProvider.SampleTestUser;
            var userRoles = new List<string> { "User" };

            _userRepositoryMock
                .Setup(m => m.ListUserRolesAsync(testUser, token))
                .ReturnsAsync(userRoles);

            var validToken = await _service.GenerateAccessTokenAsync(testUser, token);

            // Act
            var principal = _service.GetPrincipalFromToken(validToken);

            // Assert
            principal.Should().NotBeNull();
            principal.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == testUser.Id.ToString());
            principal.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == testUser.Email);
            principal.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == testUser.UserName);
            principal.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "User");
        }

        [Fact]
        public async Task GetPrincipalFromToken_WhenInvalidIssuer_ThrowSecurityTokenInvalidIssuerException()
        {
            // Arrange
            var token = CancellationToken.None;
            var testUser = TestDataProvider.SampleTestUser;
            _userRepositoryMock.Setup(m => m.ListUserRolesAsync(It.IsAny<User>(), token)).ReturnsAsync(new List<string>());

            var wrongIssuerOptions = new JwtOptions
            {
                Issuer = "wrong_issuer", 
                Audience = _testJwtOptions.Audience,
                Key = _testJwtOptions.Key,
                AccessTokenExpirationMins = _testJwtOptions.AccessTokenExpirationMins,
                RefreshTokenExpirationDays = _testJwtOptions.RefreshTokenExpirationDays
            };

            var mockWrongIssuerOptions = new Mock<IOptions<JwtOptions>>();
            
            mockWrongIssuerOptions.Setup(m => m.Value).Returns(wrongIssuerOptions);
            
            var tokenServiceWithWrongIssuer = new TokenService(mockWrongIssuerOptions.Object, _userRepositoryMock.Object);
            var tokenWithWrongIssuer = await tokenServiceWithWrongIssuer.GenerateAccessTokenAsync(testUser, token);

            // Act
            Action act = () => _service.GetPrincipalFromToken(tokenWithWrongIssuer);

            //Assert
            act.Should().Throw<SecurityTokenInvalidIssuerException>();
        }
    }
}
