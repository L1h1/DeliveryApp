using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
        private readonly IAccountService _accountService;

        public AuthService(IUserRepository userRepository, SignInManager<User> signInManager, ITokenService tokenService, IMapper mapper, IAccountService accountService)
        {
            _userRepository = userRepository;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _accountService = accountService;
        }

        public async Task<UserResponseDTO> LoginUserAsync(LoginRequestDTO loginDTO, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userRepository.GetUserByEmailAsync(loginDTO.Email, cancellationToken);

            if (user is null || !(await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false)).Succeeded)
            {
                throw new UnauthorizedException("Invalid user credentials.");
            }

            var result = _mapper.Map<UserResponseDTO>(user);
            result.AccessToken = await _tokenService.GenerateAccessTokenAsync(user, cancellationToken);

            return result;
        }

        public async Task<UserResponseDTO> UpdateTokensAsync(TokenRequestDTO tokenDTO, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var principal = _tokenService.GetPrincipalFromToken(tokenDTO.AccessToken);
            var user = await _userRepository.GetUserByEmailAsync(principal.FindFirst(ClaimTypes.Email)!.Value, cancellationToken);

            if (user is null || user.RefreshToken != tokenDTO.RefreshToken)
            {
                throw new UnauthorizedException("Invalid credentials.");
            }
            else if (user.ExpiresOn < DateTime.UtcNow)
            {
                throw new TokenExpiredException("Refresh token expired.");
            }

            (user.RefreshToken, user.ExpiresOn) = _tokenService.GenerateRefreshToken();
            await _userRepository.UpdateUserAsync(user);

            var result = _mapper.Map<UserResponseDTO>(user);
            result.AccessToken = await _tokenService.GenerateAccessTokenAsync(user);

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

            var user = _mapper.Map<User>(userDTO);
            var result = await _userRepository.AddUserAsync(user, userDTO.Password, cancellationToken);

            if (!result.Succeeded)
            {
                throw new BadRequestException(string.Join('\n', result.Errors.Select(e => e.Description)));
            }

            await _userRepository.AssignRoleAsync(user, "Client");

            var accessToken = await _tokenService.GenerateAccessTokenAsync(user, cancellationToken);
            (user.RefreshToken, user.ExpiresOn) = _tokenService.GenerateRefreshToken();
            await _userRepository.UpdateUserAsync(user);

            var response = _mapper.Map<UserResponseDTO>(user);
            response.AccessToken = accessToken;

            await _accountService.GenerateEmailConfirmationTokenAsync(userDTO.Email, cancellationToken);

            return response;
        }
    }
}
