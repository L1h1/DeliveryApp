using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UserService.BLL.DTOs.Request;
using UserService.BLL.DTOs.Response;
using UserService.BLL.Exceptions;
using UserService.BLL.Interfaces;
using UserService.DAL.Interfaces.Repositories;
using UserService.DAL.Models;

namespace UserService.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            SignInManager<User> signInManager,
            ITokenService tokenService,
            IMapper mapper,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserResponseDTO> LoginUserAsync(LoginRequestDTO loginDTO, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Searching for user: @{email}", loginDTO.Email);

            var user = await _userRepository.GetUserByEmailAsync(loginDTO.Email, cancellationToken);

            if (user is null || !(await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false)).Succeeded)
            {
                throw new UnauthorizedException("Invalid user credentials.");
            }

            var result = _mapper.Map<UserResponseDTO>(user);

            _logger.LogInformation("Generating access token for user: @{email}", user.Email);

            result.AccessToken = await _tokenService.GenerateAccessTokenAsync(user, cancellationToken);

            _logger.LogInformation("Successfully authenticated user: @{email}", user.Email);

            return result;
        }

        public async Task<UserResponseDTO> UpdateTokensAsync(TokenRequestDTO tokenDTO, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Retrieving user information from token");

            var principal = _tokenService.GetPrincipalFromToken(tokenDTO.AccessToken);
            var email = principal.FindFirst(ClaimTypes.Email)!.Value;

            _logger.LogInformation("Searching for user: @{email}", email);

            var user = await _userRepository.GetUserByEmailAsync(email, cancellationToken);

            if (user is null || user.RefreshToken != tokenDTO.RefreshToken)
            {
                throw new UnauthorizedException("Invalid credentials.");
            }
            else if (user.ExpiresOn < DateTime.UtcNow)
            {
                throw new TokenExpiredException("Refresh token expired.");
            }

            _logger.LogInformation("Processing token refresh for user: @{email}", email);

            (user.RefreshToken, user.ExpiresOn) = _tokenService.GenerateRefreshToken();
            await _userRepository.UpdateUserAsync(user, cancellationToken);

            var result = _mapper.Map<UserResponseDTO>(user);
            result.AccessToken = await _tokenService.GenerateAccessTokenAsync(user, cancellationToken);

            _logger.LogInformation("Successfully updated tokens for user: @{email}", email);

            return result;
        }

        public async Task<UserResponseDTO> RegisterUserAsync(RegisterRequestDTO userDTO, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var existingUser = await _userRepository.GetUserByEmailAsync(userDTO.Email, cancellationToken);

            if (existingUser is not null)
            {
                throw new BadRequestException("User with given email already exists.");
            }

            _logger.LogInformation("Registering new user: @{email}", userDTO.Email);

            var user = _mapper.Map<User>(userDTO);
            var result = await _userRepository.AddUserAsync(user, userDTO.Password, cancellationToken);

            if (!result.Succeeded)
            {
                throw new BadRequestException(string.Join('\n', result.Errors.Select(e => e.Description)));
            }

            _logger.LogInformation("Assigning default role to user: @{}", user.Email);

            await _userRepository.AssignRoleAsync(user, "Client", cancellationToken);

            _logger.LogInformation("Initializing access token generation for user: @{email}", user.Email);

            var accessToken = await _tokenService.GenerateAccessTokenAsync(user, cancellationToken);
            (user.RefreshToken, user.ExpiresOn) = _tokenService.GenerateRefreshToken();
            await _userRepository.UpdateUserAsync(user, cancellationToken);

            var response = _mapper.Map<UserResponseDTO>(user);
            response.AccessToken = accessToken;

            _logger.LogInformation("Successfully registered new user: @{email}", user.Email);

            return response;
        }
    }
}
