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

            // Try port 587 first, then port 465 if it fails
            var portsToTry = new[] { smtpPort, 465, 587 }.Distinct().ToArray();
            Exception? lastException = null;

            foreach (var port in portsToTry)
            {
                try
                {
                    Console.WriteLine($"📧 Attempting connection to {smtpHost}:{port}...");
                    var startTime = DateTime.UtcNow;

                    using var smtpClient = new SmtpClient(smtpHost)
                    {
                        Port = port,
                        Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                        EnableSsl = true,
                        Timeout = 20000, // 20 seconds timeout
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                    };

                    using var mailMessage = new MailMessage
                    {
                        From = new MailAddress(fromEmail!, fromName),
                        Subject = subject,
                        Body = message,
                        IsBodyHtml = isHtml,
                    };
                    mailMessage.To.Add(toEmail);

                    await smtpClient.SendMailAsync(mailMessage);
                    
                    var elapsed = (DateTime.UtcNow - startTime).TotalSeconds;
                    Console.WriteLine($"✅ Email sent successfully via port {port} in {elapsed:F2} seconds");
                    return; // Success! Exit the method
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    Console.WriteLine($"❌ Failed on port {port}: {ex.Message}");
                    
                    // If this isn't the last port, try the next one
                    if (port != portsToTry.Last())
                    {
                        Console.WriteLine($"🔄 Retrying with next port...");
                        await Task.Delay(1000); // Wait 1 second before retry
                    }
                }
            }

            // All ports failed
            Console.WriteLine($"❌ All SMTP ports failed. Last error: {lastException?.Message}");
            throw new InvalidOperationException($"Failed to send email after trying ports: {string.Join(", ", portsToTry)}", lastException);
        }
    }
}
