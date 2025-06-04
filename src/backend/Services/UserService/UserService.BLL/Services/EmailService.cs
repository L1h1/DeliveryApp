using System.Text;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using UserService.BLL.Constants;
using UserService.BLL.Interfaces;

namespace UserService.BLL.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendChangeEmailTokenAsync(string userId, string email, string token, CancellationToken cancellationToken = default)
        {
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmationEmail = $"{_configuration["BaseUrl"]}/account/confirm-email-change/{userId}/{email}/{encodedToken}";
            await SendEmailAsync(email, EmailConstants.EmailChange, confirmationEmail);
        }

        public async Task SendConfirmationEmailAsync(string email, string token, CancellationToken cancellationToken = default)
        {
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmationEmail = $"{_configuration["BaseUrl"]}/account/email-confirmation/{email}/{encodedToken}";
            await SendEmailAsync(email, EmailConstants.EmailConfirmation, confirmationEmail);
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using var client = new SmtpClient();
            client.AuthenticationMechanisms.Remove("NTLM");
            var host = _configuration.GetValue<string>("EmailSettings:Host");
            var port = _configuration.GetValue<int>("EmailSettings:Port");
            var username = _configuration.GetValue<string>("EmailSettings:Credentials:Username");
            var password = _configuration.GetValue<string>("EmailSettings:Credentials:Password");

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
        }

        public async Task SendResetPasswordEmailAsync(string email, string token, CancellationToken cancellationToken = default)
        {
            await SendEmailAsync(email, EmailConstants.PasswordReset, token);
        }
    }
}
