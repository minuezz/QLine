using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using QLine.Application.Abstractions;

namespace QLine.Infrastructure.Persistence.Notifications
{
    public class EmailSender : IEmailSender
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _fromEmail;

        public EmailSender(IConfiguration config)
        {
            _host = config["Email:Host"] ?? "localhost";
            _port = int.Parse(config["Email:Port"] ?? "1025");
            _fromEmail = config["Email:From"] ?? "no-reply@qline.local";
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using var client = new SmtpClient(_host, _port);
            client.EnableSsl = false;
            client.Credentials = CredentialCache.DefaultNetworkCredentials;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail, "QLine System"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
        }
    }
}