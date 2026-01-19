using MailKit.Security;
using CaptainTrackBackend.Application.Abstraction.Interface.Services;
using CaptainTrackBackend.Application.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using System.Net;

namespace CaptainTrackBackend.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private const int DefaultTimeoutSeconds = 30;
        private const int MaxRetries = 3;
        
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

            // Get timeout from configuration (default: 30 seconds)
            var timeoutStr = _configuration["SmtpSettings:Timeout"] 
                ?? Environment.GetEnvironmentVariable("SMTP_TIMEOUT");
            if (!int.TryParse(timeoutStr, out int timeoutSeconds) || timeoutSeconds <= 0)
            {
                timeoutSeconds = DefaultTimeoutSeconds;
            }

            // Determine secure socket options based on port
            var secureSocketOptions = port == 465 
                ? SecureSocketOptions.SslOnConnect 
                : SecureSocketOptions.StartTls;

            _logger.LogInformation("Attempting to send email to {To} via {Host}:{Port} (timeout: {Timeout}s)", 
                emailDto.To, host, port, timeoutSeconds);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromEmail, fromEmail));
            message.To.Add(MailboxAddress.Parse(emailDto.To));
            message.Subject = emailDto.Subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = emailDto.Body
            };
            message.Body = bodyBuilder.ToMessageBody();

            // Retry logic for transient failures
            Exception? lastException = null;
            for (int attempt = 1; attempt <= MaxRetries; attempt++)
            {
                using var client = new SmtpClient();
                try
                {
                    // Set timeout
                    client.Timeout = timeoutSeconds * 1000; // Convert to milliseconds

                    _logger.LogInformation("Email send attempt {Attempt}/{MaxRetries} to {To}", 
                        attempt, MaxRetries, emailDto.To);

                    // Connect with timeout
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
                    await client.ConnectAsync(host, port, secureSocketOptions, cts.Token);
                    
                    await client.AuthenticateAsync(username, password, cts.Token);
                    await client.SendAsync(message, cts.Token);
                    
                    _logger.LogInformation("Email sent successfully to {To} on attempt {Attempt}", 
                        emailDto.To, attempt);
                    
                    return new Response<EmailDto>
                    {
                        Message = "Email sent successfully",
                        Success = true,
                        Data = emailDto
                    };
                }
                catch (OperationCanceledException ex) when (ex.InnerException is TimeoutException)
                {
                    lastException = ex;
                    _logger.LogWarning("Email send attempt {Attempt}/{MaxRetries} timed out for {To}", 
                        attempt, MaxRetries, emailDto.To);
                    
                    if (attempt < MaxRetries)
                    {
                        var delay = TimeSpan.FromSeconds(2 * attempt); // Exponential backoff
                        _logger.LogInformation("Retrying after {Delay} seconds...", delay.TotalSeconds);
                        await Task.Delay(delay);
                    }
                }
                catch (TimeoutException ex)
                {
                    lastException = ex;
                    _logger.LogWarning("Email send attempt {Attempt}/{MaxRetries} timed out for {To}: {Message}", 
                        attempt, MaxRetries, emailDto.To, ex.Message);
                    
                    if (attempt < MaxRetries)
                    {
                        var delay = TimeSpan.FromSeconds(2 * attempt);
                        _logger.LogInformation("Retrying after {Delay} seconds...", delay.TotalSeconds);
                        await Task.Delay(delay);
                    }
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    _logger.LogError(ex, "Email send attempt {Attempt}/{MaxRetries} failed for {To}: {Message}", 
                        attempt, MaxRetries, emailDto.To, ex.Message);
                    
                    // Don't retry on authentication errors or invalid email
                    if (ex.Message.Contains("authentication", StringComparison.OrdinalIgnoreCase) ||
                        ex.Message.Contains("invalid", StringComparison.OrdinalIgnoreCase) ||
                        ex.Message.Contains("not allowed", StringComparison.OrdinalIgnoreCase))
                    {
                        break; // Don't retry authentication errors
                    }
                    
                    if (attempt < MaxRetries)
                    {
                        var delay = TimeSpan.FromSeconds(2 * attempt);
                        _logger.LogInformation("Retrying after {Delay} seconds...", delay.TotalSeconds);
                        await Task.Delay(delay);
                    }
                }
                finally
                {
                    if (client.IsConnected)
                    {
                        try
                        {
                            await client.DisconnectAsync(true);
                        }
                        catch { /* Ignore disconnect errors */ }
                    }
                }
            }

            // All retries failed
            var errorMessage = lastException is TimeoutException || lastException is OperationCanceledException
                ? $"Email sending timed out after {MaxRetries} attempts. This may be due to network restrictions. Consider using a cloud email service like SendGrid or Mailgun."
                : $"Failed to send email after {MaxRetries} attempts: {lastException?.Message}";

            _logger.LogError("Failed to send email to {To} after {MaxRetries} attempts. Last error: {Error}", 
                emailDto.To, MaxRetries, lastException?.Message);

            return new Response<EmailDto>
            {
                Message = errorMessage,
                Success = false,
                Data = null
            };
        }
    }
}
