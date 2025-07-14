using System.Text;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using UserService.BLL.Constants;
using UserService.BLL.Interfaces;
using UserService.DAL.Options;

namespace UserService.BLL.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailOptions _emailOptions;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailOptions> emailOptions, ILogger<EmailService> logger)
        {
            _emailOptions = emailOptions.Value;
            _logger = logger;
        }

        public async Task SendChangeEmailTokenAsync(string userId, string email, string token, CancellationToken cancellationToken = default)
        {
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmationEmail = $"{_emailOptions.BaseUrl}/account/email/change/confirmation/{userId}/{email}/{encodedToken}";
            await SendEmailAsync(email, EmailConstants.EmailChange, confirmationEmail);
        }

        public async Task SendConfirmationEmailAsync(string email, string token, CancellationToken cancellationToken = default)
        {
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmationEmail = $"{_emailOptions.BaseUrl}/account/email/confirmation/{email}/{encodedToken}";
            await SendEmailAsync(email, EmailConstants.EmailConfirmation, confirmationEmail);
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogInformation("Sending email to: @{email}", email);

            using var client = new SmtpClient();
            client.AuthenticationMechanisms.Remove("NTLM");
            var host = _emailOptions.Host;
            var port = _emailOptions.Port;
            var username = _emailOptions.Username;
            var password = _emailOptions.Password;

            using var body = new TextPart(TextFormat.Html);
            body.Text = htmlMessage;

            using var message = new MimeMessage();
            message.From.Add(new MailboxAddress(null, username));
            message.To.Add(new MailboxAddress(null, email));
            message.Subject = subject;
            message.Body = body;

            await client.ConnectAsync(host, port);
            await client.AuthenticateAsync(username, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email successfully sent to: @{email}", email);
        }

        public async Task SendResetPasswordEmailAsync(string email, string token, CancellationToken cancellationToken = default)
        {
            await SendEmailAsync(email, EmailConstants.PasswordReset, token);
        }
    }
}
