using MailKit.Security;
using CaptainTrackBackend.Application.Abstraction.Interface.Services;
using CaptainTrackBackend.Application.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;

namespace CaptainTrackBackend.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        
        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        
        public async Task<Response<EmailDto>> SendEmailAsync(EmailDto emailDto)
        {
            try
            {
                // Validate email format
                var mailAddress = new System.Net.Mail.MailAddress(emailDto.To);
            }
            catch (FormatException)
            {
                _logger.LogWarning("Invalid email address format: {Email}", emailDto.To);
                return new Response<EmailDto>
                {
                    Message = "Invalid email address format.",
                    Success = false,
                    Data = null
                };
            }
            
            // Get SMTP settings from configuration (supports environment variables)
            // Environment variables override appsettings.json
            var host = _configuration["SmtpSettings:Host"] 
                ?? Environment.GetEnvironmentVariable("SMTP_HOST");
            var portStr = _configuration["SmtpSettings:Port"] 
                ?? Environment.GetEnvironmentVariable("SMTP_PORT");
            var username = _configuration["SmtpSettings:Username"] 
                ?? Environment.GetEnvironmentVariable("SMTP_USERNAME");
            var password = _configuration["SmtpSettings:Password"] 
                ?? Environment.GetEnvironmentVariable("SMTP_PASSWORD");
            var fromEmail = _configuration["SmtpSettings:FromEmail"] 
                ?? Environment.GetEnvironmentVariable("SMTP_FROM_EMAIL");

            // Validate required settings
            if (string.IsNullOrWhiteSpace(host))
            {
                _logger.LogError("SMTP Host is not configured. Please set SmtpSettings:Host or SMTP_HOST environment variable.");
                return new Response<EmailDto>
                {
                    Message = "SMTP configuration is missing. Please contact administrator.",
                    Success = false,
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogError("SMTP credentials are not configured. Please set SmtpSettings:Username/Password or SMTP_USERNAME/SMTP_PASSWORD environment variables.");
                return new Response<EmailDto>
                {
                    Message = "SMTP credentials are missing. Please contact administrator.",
                    Success = false,
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(fromEmail))
            {
                _logger.LogWarning("SMTP FromEmail is not configured, using Username as FromEmail");
                fromEmail = username;
            }

            if (!int.TryParse(portStr, out int port))
            {
                _logger.LogWarning("Invalid SMTP port '{Port}', defaulting to 587", portStr);
                port = 587; // Default Gmail port
            }

            _logger.LogInformation("Attempting to send email to {To} via {Host}:{Port}", emailDto.To, host, port);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromEmail, fromEmail));
            message.To.Add(MailboxAddress.Parse(emailDto.To));
            message.Subject = emailDto.Subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = emailDto.Body
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(username, password);
                await client.SendAsync(message);
                _logger.LogInformation("Email sent successfully to {To}", emailDto.To);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}. Error: {Message}", emailDto.To, ex.Message);
                return new Response<EmailDto>
                {
                    Message = $"Failed to send email: {ex.Message}",
                    Success = false,
                    Data = null
                };
            }
            finally
            {
                if (client.IsConnected)
                {
                    await client.DisconnectAsync(true);
                }
            }

            return new Response<EmailDto>
            {
                Message = "Email sent successfully",
                Success = true,
                Data = emailDto
            };
        }
    }
}
