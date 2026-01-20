using MailKit.Security;
using CaptainTrackBackend.Application.Abstraction.Interface.Services;
using CaptainTrackBackend.Application.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using System.Net;
using SendGrid;
using SendGrid.Helpers.Mail;

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

            // Check if SendGrid API key is configured
            // Priority: Environment variable > appsettings.json
            // Environment variables should override config files (especially for production)
            var sendGridApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY")
                ?? _configuration["SendGrid:ApiKey"];
            
            // Check if it's a placeholder value from appsettings.json
            if (!string.IsNullOrWhiteSpace(sendGridApiKey) && 
                (sendGridApiKey.Contains("YOUR_SENDGRID", StringComparison.OrdinalIgnoreCase) ||
                 sendGridApiKey.Contains("PLACEHOLDER", StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning("SendGrid API key appears to be a placeholder value. Checking environment variable...");
                sendGridApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            }
            
            if (!string.IsNullOrWhiteSpace(sendGridApiKey))
            {
                // Trim whitespace that might have been accidentally added
                sendGridApiKey = sendGridApiKey.Trim();
                
                // Log which source was used (for debugging)
                var source = Environment.GetEnvironmentVariable("SENDGRID_API_KEY") != null 
                    ? "environment variable" 
                    : "appsettings.json";
                _logger.LogInformation("SendGrid API key loaded from {Source}", source);
                
                // Validate API key format (should start with "SG.")
                if (!sendGridApiKey.StartsWith("SG.", StringComparison.OrdinalIgnoreCase))
                {
                    // Show first 10 characters (masked) for debugging
                    var maskedKey = sendGridApiKey.Length > 10 
                        ? sendGridApiKey.Substring(0, 10) + "..." 
                        : sendGridApiKey;
                    _logger.LogWarning(
                        "SendGrid API key format appears invalid (should start with 'SG.'). " +
                        "Key starts with: '{MaskedKey}' (length: {Length} chars, source: {Source}). " +
                        "Please check your SENDGRID_API_KEY environment variable in Render. Falling back to SMTP.",
                        maskedKey, sendGridApiKey.Length, source);
                }
                else
                {
                    _logger.LogInformation("SendGrid API key found (length: {Length} chars, starts with 'SG.', source: {Source}). Using SendGrid API.", 
                        sendGridApiKey.Length, source);
                    return await SendEmailViaSendGridAsync(emailDto, sendGridApiKey);
                }
            }
            else
            {
                _logger.LogInformation("SendGrid API key not configured. Falling back to SMTP.");
            }

            // Fallback to SMTP
            return await SendEmailViaSmtpAsync(emailDto);
        }

        private async Task<Response<EmailDto>> SendEmailViaSendGridAsync(EmailDto emailDto, string apiKey)
        {
            try
            {
                _logger.LogInformation("Sending email to {To} via SendGrid API", emailDto.To);

                // Validate API key format
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    _logger.LogError("SendGrid API key is null or empty");
                    return new Response<EmailDto>
                    {
                        Message = "SendGrid API key is not configured.",
                        Success = false,
                        Data = null
                    };
                }

                if (!apiKey.StartsWith("SG.", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogError("SendGrid API key format is invalid. API key should start with 'SG.'");
                    return new Response<EmailDto>
                    {
                        Message = "SendGrid API key format is invalid. Please check your SENDGRID_API_KEY environment variable.",
                        Success = false,
                        Data = null
                    };
                }

                var client = new SendGridClient(apiKey);
                
                // Priority: Environment variables > appsettings.json
                // Environment variables should override config files (especially for production)
                var fromEmail = Environment.GetEnvironmentVariable("SENDGRID_FROM_EMAIL")
                    ?? Environment.GetEnvironmentVariable("SMTP_FROM_EMAIL")
                    ?? _configuration["SendGrid:FromEmail"]
                    ?? _configuration["SmtpSettings:FromEmail"];
                
                // Check if it's a placeholder value from appsettings.json
                if (!string.IsNullOrWhiteSpace(fromEmail) && 
                    (fromEmail.Contains("yourdomain.com", StringComparison.OrdinalIgnoreCase) ||
                     fromEmail.Contains("YOUR_EMAIL", StringComparison.OrdinalIgnoreCase) ||
                     fromEmail.Contains("PLACEHOLDER", StringComparison.OrdinalIgnoreCase)))
                {
                    _logger.LogWarning("SendGrid FromEmail appears to be a placeholder value. Checking environment variables...");
                    fromEmail = Environment.GetEnvironmentVariable("SENDGRID_FROM_EMAIL")
                        ?? Environment.GetEnvironmentVariable("SMTP_FROM_EMAIL");
                }
                
                // Log which source was used (for debugging)
                if (!string.IsNullOrWhiteSpace(fromEmail))
                {
                    var source = Environment.GetEnvironmentVariable("SENDGRID_FROM_EMAIL") != null 
                        ? "SENDGRID_FROM_EMAIL environment variable"
                        : Environment.GetEnvironmentVariable("SMTP_FROM_EMAIL") != null
                            ? "SMTP_FROM_EMAIL environment variable"
                            : "appsettings.json";
                    _logger.LogInformation("SendGrid FromEmail loaded from {Source}: {FromEmail}", source, fromEmail);
                }
                
                if (string.IsNullOrWhiteSpace(fromEmail))
                {
                    _logger.LogError("SendGrid FromEmail is not configured. Please set SENDGRID_FROM_EMAIL environment variable in Render.");
                    return new Response<EmailDto>
                    {
                        Message = "SendGrid FromEmail is not configured. Please set SENDGRID_FROM_EMAIL environment variable in Render.",
                        Success = false,
                        Data = null
                    };
                }

                var from = new EmailAddress(fromEmail);
                var to = new EmailAddress(emailDto.To);
                var subject = emailDto.Subject;
                var htmlContent = emailDto.Body;
                var plainTextContent = System.Text.RegularExpressions.Regex.Replace(htmlContent, "<[^>]*>", ""); // Strip HTML for plain text

                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                
                var response = await client.SendEmailAsync(msg);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Email sent successfully to {To} via SendGrid", emailDto.To);
                    return new Response<EmailDto>
                    {
                        Message = "Email sent successfully",
                        Success = true,
                        Data = emailDto
                    };
                }
                else
                {
                    var responseBody = await response.Body.ReadAsStringAsync();
                    _logger.LogError("SendGrid API error. Status: {StatusCode}, Body: {Body}", 
                        response.StatusCode, responseBody);
                    
                    // Parse error message from response body to provide specific guidance
                    string errorMessage;
                    
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        errorMessage = "SendGrid API key is invalid, expired, or revoked. Please verify your SENDGRID_API_KEY environment variable in Render dashboard and ensure the API key has 'Mail Send' permissions.";
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        // Check if it's a sender identity issue vs API permission issue
                        if (responseBody.Contains("from address does not match a verified Sender Identity", StringComparison.OrdinalIgnoreCase) ||
                            responseBody.Contains("sender identity", StringComparison.OrdinalIgnoreCase))
                        {
                            errorMessage = $"SendGrid sender email '{fromEmail}' is not verified. " +
                                $"Please verify this email address in SendGrid: https://app.sendgrid.com/settings/sender_auth " +
                                $"Then ensure SENDGRID_FROM_EMAIL in Render matches the verified email address.";
                        }
                        else
                        {
                            errorMessage = "SendGrid API key does not have required permissions. Please ensure the API key has 'Mail Send' permission in SendGrid dashboard.";
                        }
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        errorMessage = $"SendGrid request is invalid: {responseBody}";
                    }
                    else
                    {
                        errorMessage = $"SendGrid API error: {response.StatusCode}. {responseBody}";
                    }
                    
                    _logger.LogError("SendGrid error details: {ErrorMessage}", errorMessage);
                    
                    return new Response<EmailDto>
                    {
                        Message = errorMessage,
                        Success = false,
                        Data = null
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email via SendGrid to {To}: {Message}", emailDto.To, ex.Message);
                return new Response<EmailDto>
                {
                    Message = $"Failed to send email via SendGrid: {ex.Message}",
                    Success = false,
                    Data = null
                };
            }
        }

        private async Task<Response<EmailDto>> SendEmailViaSmtpAsync(EmailDto emailDto)
        {
            
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
