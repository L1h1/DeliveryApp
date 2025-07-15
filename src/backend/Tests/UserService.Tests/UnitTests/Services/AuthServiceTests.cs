using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;
using UserService.BLL.DTOs.Request;
using UserService.BLL.DTOs.Response;
using UserService.BLL.Exceptions;
using UserService.BLL.Interfaces;
using UserService.BLL.Services;
using UserService.DAL.Interfaces.Repositories;
using UserService.DAL.Models;

namespace UserService.Tests.UnitTests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _userRepositoryMock = new();

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<User>>(),
                new List<IUserValidator<User>>(),
                new List<IPasswordValidator<User>>(),
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<User>>>()
            );

            _signInManagerMock = new Mock<SignInManager<User>>(
                userManagerMock.Object,
                Mock.Of<Microsoft.AspNetCore.Http.IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<User>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<ILogger<SignInManager<User>>>(),
                Mock.Of<IAuthenticationSchemeProvider>(),
                Mock.Of<IUserConfirmation<User>>()
            );

            _tokenServiceMock = new();
            _mapperMock = new();
            _loggerMock = new();

            _service = new(
                _userRepositoryMock.Object,
                _signInManagerMock.Object,
                _tokenServiceMock.Object,
                _mapperMock.Object,
                 _loggerMock.Object);
        }

        [Fact]
        public async Task LoginUserAsync_WhenValidCredentialsProvided_ReturnsTokens()
        {
            //Arrange
            var token = CancellationToken.None;
            var loginRequest = new LoginRequestDTO {
                Email = "test@example.com",
                Password = "Password123!" };
            var user = TestDataProvider.SampleTestUser;
            var accessToken = "test_access_token";
            var refreshToken = "test_refresh_token";
            var userResponse = new UserResponseDTO {
                Email = user.Email,
                UserName = user.UserName,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };

            _userRepositoryMock.Setup(m => m.GetUserByEmailAsync(loginRequest.Email, token)).ReturnsAsync(user);
            _signInManagerMock.Setup(m => m.CheckPasswordSignInAsync(user, loginRequest.Password, false)).ReturnsAsync(SignInResult.Success);
            _mapperMock.Setup(m => m.Map<UserResponseDTO>(user)).Returns(userResponse);
            _tokenServiceMock.Setup(m => m.GenerateAccessTokenAsync(user, token)).ReturnsAsync(accessToken);
            _tokenServiceMock.Setup(m => m.GenerateRefreshToken()).Returns((refreshToken, DateTime.Now));

            //Act
            var result = await _service.LoginUserAsync(loginRequest, CancellationToken.None);

            //Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(loginRequest.Email);
            result.AccessToken.Should().Be(accessToken);
            result.RefreshToken.Should().Be(refreshToken);
            _userRepositoryMock.Verify(m => m.GetUserByEmailAsync(loginRequest.Email, token), Times.Once);
            _signInManagerMock.Verify(m => m.CheckPasswordSignInAsync(user, loginRequest.Password, false), Times.Once);
            _mapperMock.Verify(m => m.Map<UserResponseDTO>(user), Times.Once);
            _tokenServiceMock.Verify(m => m.GenerateAccessTokenAsync(user, token), Times.Once);
        }

        [Fact]
        public async Task LoginUserAsync_WhenInvalidCredentialsProvided_ThrowsUnauthorizedException()
        {
            //Arrange
            var token = CancellationToken.None;
            var loginRequest = new LoginRequestDTO {
                Email = "test@example.com", 
                Password = "Password123!" };
            var user = TestDataProvider.SampleTestUser;
            _userRepositoryMock.Setup(m => m.GetUserByEmailAsync(loginRequest.Email, token)).ReturnsAsync(user);
            _signInManagerMock.Setup(m => m.CheckPasswordSignInAsync(user, loginRequest.Password, false)).ReturnsAsync(SignInResult.Failed);

            //Act
            var result = () => _service.LoginUserAsync(loginRequest, CancellationToken.None);

            //Assert
            result.Should().ThrowAsync<UnauthorizedException>();
            _userRepositoryMock.Verify(m => m.GetUserByEmailAsync(loginRequest.Email, token), Times.Once);
            _signInManagerMock.Verify(m => m.CheckPasswordSignInAsync(user, loginRequest.Password, false), Times.Once);
            _mapperMock.Verify(m => m.Map<UserResponseDTO>(user), Times.Never);
            _tokenServiceMock.Verify(m => m.GenerateAccessTokenAsync(user, token), Times.Never);
        }

        [Fact]
        public async Task UpdateTokensAsync_WhenValidTokenProvided_ReturnsNewTokens()
        {
            //Arrange
            var tokenRequest = new TokenRequestDTO() {
                AccessToken = "old_access_token", 
                RefreshToken = "refresh_token" };
            var token = CancellationToken.None;
            var user  = TestDataProvider.SampleTestUser;
            user.ExpiresOn = DateTime.UtcNow.AddDays(1);
            user.RefreshToken = tokenRequest.RefreshToken;
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, user.Email)
            }));
            var newRefreshToken = "new_refresh_token";
            var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            var newAccessToken = "new_access_token";
            var userResponse = new UserResponseDTO {
                Email = user.Email,
                UserName = user.UserName,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            };

            _tokenServiceMock.Setup(t => t.GetPrincipalFromToken(tokenRequest.AccessToken)).Returns(principal);
            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(user.Email, token)).ReturnsAsync(user);
            _tokenServiceMock.Setup(t => t.GenerateRefreshToken()).Returns((newRefreshToken, newRefreshTokenExpiry));
            _userRepositoryMock.Setup(r => r.UpdateUserAsync(user, token)).ReturnsAsync(new IdentityResult());
            _mapperMock.Setup(m => m.Map<UserResponseDTO>(user)).Returns(userResponse);
            _tokenServiceMock.Setup(t => t.GenerateAccessTokenAsync(user, token)).ReturnsAsync(newAccessToken);

            //Act
            var result = await _service.UpdateTokensAsync(tokenRequest, token);

            //Assert
            _tokenServiceMock.Verify(t => t.GetPrincipalFromToken(tokenRequest.AccessToken), Times.Once);
            _userRepositoryMock.Verify(r => r.GetUserByEmailAsync(user.Email, token), Times.Once);
            _tokenServiceMock.Verify(t => t.GenerateRefreshToken(), Times.Once);
            _userRepositoryMock.Verify(r => r.UpdateUserAsync(user, token), Times.Once);
            _mapperMock.Verify(m => m.Map<UserResponseDTO>(user), Times.Once);
            _tokenServiceMock.Verify(t => t.GenerateAccessTokenAsync(user, token), Times.Once);
        }

        [Fact]
        public async Task UpdateTokensAsync_WhenInvalidAccessTokenProvided_ThrowsUnauthorizedException()
        {
            //Arrange
            var tokenRequest = new TokenRequestDTO() {
                AccessToken = "old_access_token", 
                RefreshToken = "refresh_token" };
            var token = CancellationToken.None;
            var user = TestDataProvider.SampleTestUser;
            user.ExpiresOn = DateTime.UtcNow.AddDays(1);
            user.RefreshToken = "INVALID";
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, user.Email)
            }));
            var newRefreshToken = "new_refresh_token";
            var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            var newAccessToken = "new_access_token";
            var userResponse = new UserResponseDTO
            {
                Email = user.Email,
                UserName = user.UserName,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            };

            _tokenServiceMock.Setup(t => t.GetPrincipalFromToken(tokenRequest.AccessToken)).Returns(principal);
            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(user.Email, token)).ReturnsAsync(user);

            //Act
            var result = () => _service.UpdateTokensAsync(tokenRequest, token);

            //Assert
            result.Should().ThrowAsync<UnauthorizedException>();
            _tokenServiceMock.Verify(t => t.GetPrincipalFromToken(tokenRequest.AccessToken), Times.Once);
            _userRepositoryMock.Verify(r => r.GetUserByEmailAsync(user.Email, token), Times.Once);
            _tokenServiceMock.Verify(t => t.GenerateRefreshToken(), Times.Never);
            _userRepositoryMock.Verify(r => r.UpdateUserAsync(user, token), Times.Never);
            _mapperMock.Verify(m => m.Map<UserResponseDTO>(user), Times.Never);
            _tokenServiceMock.Verify(t => t.GenerateAccessTokenAsync(user, token), Times.Never);
        }

        [Fact]
        public async Task UpdateTokensAsync_WhenExpiredRefreshTokenProvided_ThrowsTokenExpiredException()
        {
            //Arrange
            var tokenRequest = new TokenRequestDTO() {
                AccessToken = "old_access_token",
                RefreshToken = "refresh_token" };
            var token = CancellationToken.None;
            var user = TestDataProvider.SampleTestUser;
            user.ExpiresOn = DateTime.UtcNow.AddDays(-1);
            user.RefreshToken = tokenRequest.RefreshToken;
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, user.Email)
            }));
            var newRefreshToken = "new_refresh_token";
            var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            var newAccessToken = "new_access_token";
            var userResponse = new UserResponseDTO
            {
                Email = user.Email,
                UserName = user.UserName,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            };

            _tokenServiceMock.Setup(t => t.GetPrincipalFromToken(tokenRequest.AccessToken)).Returns(principal);
            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(user.Email, token)).ReturnsAsync(user);

            //Act
            var result = () => _service.UpdateTokensAsync(tokenRequest, token);

            //Assert
            result.Should().ThrowAsync<TokenExpiredException>();
            _tokenServiceMock.Verify(t => t.GetPrincipalFromToken(tokenRequest.AccessToken), Times.Once);
            _userRepositoryMock.Verify(r => r.GetUserByEmailAsync(user.Email, token), Times.Once);
            _tokenServiceMock.Verify(t => t.GenerateRefreshToken(), Times.Never);
            _userRepositoryMock.Verify(r => r.UpdateUserAsync(user, token), Times.Never);
            _mapperMock.Verify(m => m.Map<UserResponseDTO>(user), Times.Never);
            _tokenServiceMock.Verify(t => t.GenerateAccessTokenAsync(user, token), Times.Never);
        }

        [Fact]
        public async Task RegisterUserAsync_WhenValidDataProvided_ReturnsSuccessResult()
        {
            //Arrange
            var token = CancellationToken.None;
            var user = TestDataProvider.SampleTestUser;
            var registerRequest = new RegisterRequestDTO {
                Email = user.Email,
                PhoneNumber = "+375441111111",
                UserName = user.Email,
                Password = "NewPassword123!",
            };
            var accessToken = "new_access_token";
            var refreshToken = "new_refresh_token";
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            var userResponse = new UserResponseDTO {
                Email = user.Email,
                UserName = user.UserName,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };

            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(registerRequest.Email, token)).ReturnsAsync((User)null);
            _mapperMock.Setup(m => m.Map<User>(registerRequest)).Returns(user);
            _userRepositoryMock.Setup(r => r.AddUserAsync(user, registerRequest.Password, token)).ReturnsAsync(IdentityResult.Success);
            _userRepositoryMock.Setup(r => r.AssignRoleAsync(user, "Client", token)).ReturnsAsync(IdentityResult.Success);
            _tokenServiceMock.Setup(t => t.GenerateAccessTokenAsync(user, token)).ReturnsAsync(accessToken);
            _tokenServiceMock.Setup(t => t.GenerateRefreshToken()).Returns((refreshToken, refreshTokenExpiry));
            _userRepositoryMock.Setup(r => r.UpdateUserAsync(user, token)).ReturnsAsync(IdentityResult.Success);
            _mapperMock.Setup(m => m.Map<UserResponseDTO>(user)).Returns(userResponse);

            //Act
            var result = await _service.RegisterUserAsync(registerRequest, CancellationToken.None);

            //Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(registerRequest.Email);
            result.AccessToken.Should().Be(accessToken);
            user.RefreshToken.Should().Be(refreshToken); 
            user.ExpiresOn.Should().Be(refreshTokenExpiry); 

            _userRepositoryMock.Verify(r => r.GetUserByEmailAsync(registerRequest.Email, token), Times.Once);
            _mapperMock.Verify(m => m.Map<User>(registerRequest), Times.Once);
            _userRepositoryMock.Verify(r => r.AddUserAsync(user, registerRequest.Password, token), Times.Once);
            _userRepositoryMock.Verify(r => r.AssignRoleAsync(user, "Client", token), Times.Once);
            _tokenServiceMock.Verify(t => t.GenerateAccessTokenAsync(user, token), Times.Once);
            _tokenServiceMock.Verify(t => t.GenerateRefreshToken(), Times.Once);
            _userRepositoryMock.Verify(r => r.UpdateUserAsync(user, token), Times.Once);
            _mapperMock.Verify(m => m.Map<UserResponseDTO>(user), Times.Once);
        }

        [Fact]
        public async Task RegisterUserAsync_EmailAlreadyTaken_ThrowsBadRequestException()
        {
            //Arrange
            var token = CancellationToken.None;
            var user = TestDataProvider.SampleTestUser;
            var registerRequest = new RegisterRequestDTO
            {
                Email = user.Email,
                PhoneNumber = "+375441111111",
                UserName = user.Email,
                Password = "NewPassword123!",
            };

            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(registerRequest.Email, token)).ReturnsAsync(user);

            //Act
            var result = () => _service.RegisterUserAsync(registerRequest, CancellationToken.None);

            //Assert
            result.Should().ThrowAsync<BadRequestException>();

            _userRepositoryMock.Verify(r => r.GetUserByEmailAsync(registerRequest.Email, token), Times.Once);
            _mapperMock.Verify(m => m.Map<User>(registerRequest), Times.Never);
            _userRepositoryMock.Verify(r => r.AddUserAsync(user, registerRequest.Password, token), Times.Never);
            _userRepositoryMock.Verify(r => r.AssignRoleAsync(user, "Client", token), Times.Never);
            _tokenServiceMock.Verify(t => t.GenerateAccessTokenAsync(user, token), Times.Never);
            _tokenServiceMock.Verify(t => t.GenerateRefreshToken(), Times.Never);
            _userRepositoryMock.Verify(r => r.UpdateUserAsync(user, token), Times.Never);
            _mapperMock.Verify(m => m.Map<UserResponseDTO>(user), Times.Never);
        }
    }
}
