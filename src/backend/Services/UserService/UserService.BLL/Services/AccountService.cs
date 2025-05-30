using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UserService.BLL.Constants;
using UserService.BLL.DTOs.Response;
using UserService.BLL.Exceptions;
using UserService.BLL.Interfaces;
using UserService.DAL.Interfaces.Repositories;

namespace UserService.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AccountService(IUserRepository userRepository, IEmailSender emailSender, IMapper mapper, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _emailSender = emailSender;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<IdentityResult> ConfirmEmailASync(string email, string token, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userRepository.GetUserByEmailAsync(email, cancellationToken);

            if (user is null)
            {
                throw new UnauthorizedException("User with given credentials not found.");
            }

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userRepository.ConfirmEmailASync(user, decodedToken, cancellationToken);

            if (!result.Succeeded)
            {
                throw new UnauthorizedException("Invalid confirmation token.");
            }

            return result;
        }

        public async Task GenerateEmailConfirmationTokenAsync(string email, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userRepository.GetUserByEmailAsync(email, cancellationToken);

            if (user is null)
            {
                throw new UnauthorizedException("User with given credentials not found.");
            }

            var token = await _userRepository.GenerateEmailConfirmationTokenAsync(user, cancellationToken);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmationEmail = $"{_configuration["BaseUrl"]}/account/email-confirmation/{email}/{encodedToken}";

            await _emailSender.SendEmailAsync(email, EmailConstants.EmailConfirmation, confirmationEmail);
        }

        public async Task GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userRepository.GetUserByEmailAsync(email, cancellationToken);

            if (user is null)
            {
                throw new UnauthorizedException("User with given credentials not found.");
            }

            var resetCode = await _userRepository.GeneratePasswordResetTokenAsync(user, cancellationToken);

            await _emailSender.SendEmailAsync(email, EmailConstants.PasswordReset, resetCode);
        }

        public async Task<IdentityResult> ResetPasswordAsync(string email, string resetCode, string newPassword, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userRepository.GetUserByEmailAsync(email, cancellationToken);

            if (user is null)
            {
                throw new UnauthorizedException("User with given credentials not found.");
            }

            var result = await _userRepository.ResetPasswordAsync(user, resetCode, newPassword, cancellationToken);

            return result;
        }

        public async Task<List<string>> ListRolesAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _userRepository.ListRolesAsync(cancellationToken);

            if (result.IsNullOrEmpty())
            {
                throw new NotFoundException("No roles found.");
            }

            return result;
        }

        public async Task<List<UserDetailsDTO>> ListUsersByRoleAsync(string role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _userRepository.ListUsersByRoleAsync(role, cancellationToken);

            if (result.IsNullOrEmpty())
            {
                throw new NotFoundException("No users found for the given role.");
            }

            return _mapper.Map<List<UserDetailsDTO>>(result);
        }

        public async Task GenerateEmailChangeTokenAsync(string userId, string email, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var existingUser = await _userRepository.GetUserByEmailAsync(email, cancellationToken);

            if (existingUser is not null)
            {
                throw new BadRequestException("Email already taken.");
            }

            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            var token = await _userRepository.GenerateEmailChangeTokenAsync(user, email, cancellationToken);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmationEmail = $"{_configuration["BaseUrl"]}/account/confirm-email-change/{user.Id}/{email}/{encodedToken}";

            await _emailSender.SendEmailAsync(email, EmailConstants.EmailChange, confirmationEmail);
        }

        public async Task<IdentityResult> ConfirmEmailChangeAsync(string userId, string email, string token, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userRepository.ConfirmEmailChangeAsync(user, email, decodedToken, cancellationToken);

            if (!result.Succeeded)
            {
                throw new UnauthorizedException("Invalid confirmation token.");
            }

            return result;
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            var result = await _userRepository.ChangePasswordAsync(user, currentPassword, newPassword, cancellationToken);

            return result;
        }

        public async Task<UserDetailsDTO> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);

            return _mapper.Map<UserDetailsDTO>(user);
        }
    }
}
