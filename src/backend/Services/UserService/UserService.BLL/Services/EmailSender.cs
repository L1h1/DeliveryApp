using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace UserService.BLL.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using var client = new SmtpClient();
            client.AuthenticationMechanisms.Remove("NTLM");
            var host = _configuration.GetValue<string>("EmailOptions:Host");
            var port = _configuration.GetValue<int>("EmailOptions:Port");
            var username = _configuration.GetValue<string>("EmailOptions:Credentials:Username");
            var password = _configuration.GetValue<string>("EmailOptions:Credentials:Password");

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
    }
}
