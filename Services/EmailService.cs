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

            Console.WriteLine($"📧 Starting email send to: {toEmail}");
            Console.WriteLine($"📧 SMTP Host: {smtpHost}:{smtpPort}");
            Console.WriteLine($"📧 SMTP Username: {smtpUsername}");
            Console.WriteLine($"📧 SMTP Password configured: {!string.IsNullOrEmpty(smtpPassword)}");

            if (string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
            {
                Console.WriteLine("❌ SMTP credentials are missing!");
                throw new InvalidOperationException("SMTP credentials are not configured. Please set Smtp:Username and Smtp:Password in environment variables");
            }

            var smtpClient = new SmtpClient(smtpHost)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true,
                Timeout = 10000, // 10 seconds timeout (instead of default 100 seconds)
                DeliveryMethod = SmtpDeliveryMethod.Network,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail!, fromName),
                Subject = subject,
                Body = message,
                IsBodyHtml = isHtml,
            };
            mailMessage.To.Add(toEmail);

            Console.WriteLine($"📧 Sending email via {smtpHost}...");
            var startTime = DateTime.UtcNow;
            
            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                var elapsed = (DateTime.UtcNow - startTime).TotalSeconds;
                Console.WriteLine($"✅ Email sent successfully in {elapsed:F2} seconds");
            }
            catch (Exception ex)
            {
                var elapsed = (DateTime.UtcNow - startTime).TotalSeconds;
                Console.WriteLine($"❌ Email failed after {elapsed:F2} seconds: {ex.Message}");
                throw;
            }
            finally
            {
                mailMessage.Dispose();
            }
        }
    }
}
