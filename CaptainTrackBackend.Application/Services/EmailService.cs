using MailKit.Security;
using CaptainTrackBackend.Application.Abstraction.Interface.Services;
using CaptainTrackBackend.Application.DTO;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;

namespace CaptainTrackBackend.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
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
                return new Response<EmailDto>
                {
                    Message = "Invalid email address format.",
                    Success = false,
                    Data = null
                };
                //throw new InvalidOperationException("Invalid recipient email address.");
            }
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            var host = smtpSettings["Host"];
            var portStr = smtpSettings["Port"];
            var username = smtpSettings["Username"];
            var password = smtpSettings["Password"];
            var fromEmail = smtpSettings["FromEmail"];

            if (!int.TryParse(portStr, out int port))
                throw new InvalidOperationException("Invalid SMTP port in configuration.");

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
            }
            catch (Exception ex)
            {
                return new Response<EmailDto>
                {
                    Message = $"Failed to send email: {ex.Message}",
                    Success = false,
                    Data = null
                };
                //throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
            }
            finally
            {
                await client.DisconnectAsync(true);
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
