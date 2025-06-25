using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using UserService.BLL.DTOs.Response;
using UserService.BLL.Exceptions;
using UserService.BLL.Interfaces;
using UserService.DAL.Interfaces.Repositories;

namespace UserService.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailSender;
        private readonly IMapper _mapper;
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly ILogger<AccountService> _logger;

        public AccountService(IUserRepository userRepository, IEmailService emailSender, IMapper mapper, IBackgroundJobService backgroundJobService, ILogger<AccountService> logger)
        {
            _userRepository = userRepository;
            _emailSender = emailSender;
            _mapper = mapper;
            _backgroundJobService = backgroundJobService;
            _logger = logger;
        }

        public async Task<IdentityResult> ConfirmEmailASync(string email, string token, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Searching for user: @{email}", email);

            var user = await _userRepository.GetUserByEmailAsync(email, cancellationToken);

            if (user is null)
            {
                throw new UnauthorizedException("User with given credentials not found.");
            }

            _logger.LogInformation("Processing confirmation token for user: @{email}", email);

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userRepository.ConfirmEmailASync(user, decodedToken, cancellationToken);

            if (!result.Succeeded)
            {
                throw new UnauthorizedException("Invalid confirmation token.");
            }

            _logger.LogInformation("Finished email confirmation for user: @{email}", email);

            return result;
        }

        public async Task GenerateEmailConfirmationTokenAsync(string email, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Beginning email token generation for user: @{email}", email);

            var user = await _userRepository.GetUserByEmailAsync(email, cancellationToken);

            if (user is null)
            {
                throw new UnauthorizedException("User with given credentials not found.");
            }

            var token = await _userRepository.GenerateEmailConfirmationTokenAsync(user, cancellationToken);

            _logger.LogInformation("Scheduling confirmation email job for user: @{email}", email);

            _backgroundJobService.CreateJob(() => _emailSender.SendConfirmationEmailAsync(email, token, cancellationToken));
        }

        public async Task GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Beginning password reset token generation for user: @{email}", email);

            var user = await _userRepository.GetUserByEmailAsync(email, cancellationToken);

            if (user is null)
            {
                throw new UnauthorizedException("User with given credentials not found.");
            }

            var resetCode = await _userRepository.GeneratePasswordResetTokenAsync(user, cancellationToken);

            _logger.LogInformation("Scheduling password reset email job for user: @{email}", email);

            _backgroundJobService.CreateJob(() => _emailSender.SendResetPasswordEmailAsync(email, resetCode, cancellationToken));
        }

        public async Task<IdentityResult> ResetPasswordAsync(string email, string resetCode, string newPassword, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Searching for user: @{email}", email);

            var user = await _userRepository.GetUserByEmailAsync(email, cancellationToken);

            if (user is null)
            {
                throw new UnauthorizedException("User with given credentials not found.");
            }

            var result = await _userRepository.ResetPasswordAsync(user, resetCode, newPassword, cancellationToken);

            _logger.LogInformation("Password reset successful for user: @{email}", email);

            return result;
        }

        public async Task<List<string>> ListRolesAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Retrieving role list");

            var result = await _userRepository.ListRolesAsync(cancellationToken);

            if (result.IsNullOrEmpty())
            {
                throw new NotFoundException("No roles found.");
            }

            _logger.LogInformation("Successfully retrieved role list");

            return result;
        }

        public async Task<List<UserDetailsDTO>> ListUsersByRoleAsync(string role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Retrieving all users for with role: @{role}", role);

            var result = await _userRepository.ListUsersByRoleAsync(role, cancellationToken);

            if (result.IsNullOrEmpty())
            {
                throw new NotFoundException("No users found for the given role.");
            }

            _logger.LogInformation("Successfully retrieved all users with role: @{role}", role);

            return _mapper.Map<List<UserDetailsDTO>>(result);
        }

        public async Task GenerateEmailChangeTokenAsync(string userId, string email, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Searching for user: @{email}", email);

            var existingUser = await _userRepository.GetUserByEmailAsync(email, cancellationToken);

            if (existingUser is not null)
            {
                throw new BadRequestException("Email already taken.");
            }

            _logger.LogInformation("Generating email change token for user: @{email}", email);

            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            var token = await _userRepository.GenerateEmailChangeTokenAsync(user, email, cancellationToken);

            _logger.LogInformation("Scheduling change email job for user: @{email}", email);

            _backgroundJobService.CreateJob(() => _emailSender.SendChangeEmailTokenAsync(userId, email, token, cancellationToken));
        }

        public async Task<IdentityResult> ConfirmEmailChangeAsync(string userId, string email, string token, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Processing email confirmation token for user: @{email}", email);

            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userRepository.ConfirmEmailChangeAsync(user, email, decodedToken, cancellationToken);

            if (!result.Succeeded)
            {
                throw new UnauthorizedException("Invalid confirmation token.");
            }

            _logger.LogInformation("Successfully changed email for user: @{email}", email);

            return result;
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Processing password change for user: @{userId}", userId);

            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            var result = await _userRepository.ChangePasswordAsync(user, currentPassword, newPassword, cancellationToken);

            _logger.LogInformation("Password changed successfully for user: @{userId}", userId);

            return result;
        }

        public async Task<UserDetailsDTO> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Retrieving information about user: @{userId}", userId);

            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);

            _logger.LogInformation("Successfully retrieved information about user: @{userId}", userId);

            return _mapper.Map<UserDetailsDTO>(user);
        }
    }
}
