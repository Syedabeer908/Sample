using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using WebApplication1.Email;
using WebApplication1.Settings;

namespace WebApplication1.Services
{
    public class EmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailOptions, ILogger<EmailService> logger)
        {
            _settings = emailOptions.Value;
            _logger = logger;
        }

        private async Task Send(MimeMessage email)
        {
            using var smtp = new SmtpClient();

            try
            {
                await smtp.ConnectAsync(
                    _settings.Host,
                    _settings.Port,
                    MailKit.Security.SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(_settings.Username, _settings.Password);

                await smtp.SendAsync(email);

                _logger.LogInformation("Email sent successfully to {To}", email.To);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email sending failed");
                throw;
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }

        public async Task SendEmailAsync(EmailRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ToEmail))
                return;

            var email = new MimeMessage();

            email.From.Add(new MailboxAddress("", _settings.Username));
            email.To.Add(new MailboxAddress("", request.ToEmail));
            email.Subject = request.Subject;

            email.Body = new TextPart("html")
            {
                Text = request.Body
            };

            await Send(email);
        }
    }
}
