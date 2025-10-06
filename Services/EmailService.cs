using Resend;

namespace ECommerceApi.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly IResend _resend;

        public EmailService(IConfiguration config, IResend resend)
        {
            _config = config;
            _resend = resend;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message, bool isHtml)
        {
            var fromEmail = _config["Email:FromEmail"]?.Trim() ?? "onboarding@resend.dev";
            var fromName = _config["Email:FromName"]?.Trim() ?? "ModernHub";

            Console.WriteLine($"📧 Starting email send via Resend API to: {toEmail}");
            Console.WriteLine($"📧 FromEmail raw: '{fromEmail}'");
            Console.WriteLine($"📧 FromName raw: '{fromName}'");
            
            var startTime = DateTime.UtcNow;

            try
            {
                // Format: "Name <email@domain.com>"
                var fromField = string.IsNullOrEmpty(fromName) 
                    ? fromEmail 
                    : $"{fromName} <{fromEmail}>";
                
                Console.WriteLine($"📧 From field: '{fromField}'");

                var emailMessage = new EmailMessage
                {
                    From = fromField,
                    To = toEmail,
                    Subject = subject,
                    HtmlBody = isHtml ? message : null,
                    TextBody = !isHtml ? message : null
                };

                var response = await _resend.EmailSendAsync(emailMessage);
                
                var elapsed = (DateTime.UtcNow - startTime).TotalSeconds;
                
                Console.WriteLine($"✅ Email sent successfully via Resend API in {elapsed:F2} seconds");
                Console.WriteLine($"📧 Email ID: {response}");
            }
            catch (Exception ex)
            {
                var elapsed = (DateTime.UtcNow - startTime).TotalSeconds;
                Console.WriteLine($"❌ Email failed after {elapsed:F2} seconds: {ex.Message}");
                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
