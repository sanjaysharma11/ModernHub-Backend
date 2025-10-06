using System.Net;
using System.Net.Mail;

namespace ECommerceApi.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config) => _config = config;

        public async Task SendEmailAsync(string toEmail, string subject, string message, bool isHtml)
        {
            var smtpHost = _config["Smtp:Host"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_config["Smtp:Port"] ?? "587");
            var smtpUsername = _config["Smtp:Username"];
            var smtpPassword = _config["Smtp:Password"];
            var fromEmail = _config["Smtp:FromEmail"] ?? smtpUsername;
            var fromName = _config["Smtp:FromName"] ?? "ModernHub";

            if (string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
            {
                throw new InvalidOperationException("SMTP credentials are not configured. Please set Smtp:Username and Smtp:Password in appsettings.json");
            }

            var smtpClient = new SmtpClient(smtpHost)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail!, fromName),
                Subject = subject,
                Body = message,
                IsBodyHtml = isHtml,
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
