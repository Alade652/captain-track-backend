using CaptainTrackBackend.Application.Abstraction.Interface.Services;
using Microsoft.Extensions.Configuration;

namespace CaptainTrackBackend.Persistence
{
    public class HollatagsSmsService : ISmsService
    {

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HollatagsSmsService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }


        public async Task<string> SendSmsAsync(string toPhoneNumber, string message)
        {
            if (string.IsNullOrWhiteSpace(toPhoneNumber) || !IsValidPhoneNumber(toPhoneNumber))
                throw new ArgumentException("Invalid phone number format.", nameof(toPhoneNumber));
            if (string.IsNullOrWhiteSpace(message) || message.Length > 160)
                throw new ArgumentException("Message is empty or exceeds 160 characters.", nameof(message));

            //string m = message.ToString();

            var formData = new Dictionary<string, string>
            {
                { "user", _configuration["Hollatags:User"] ?? throw new InvalidOperationException("Hollatags:User configuration missing") },
                { "pass", _configuration["Hollatags:Pass"] ?? throw new InvalidOperationException("Hollatags:Pass configuration missing") },
                { "from", _configuration["Hollatags:From"] ?? throw new InvalidOperationException("Hollatags:From configuration missing") },
                { "msg", message },
                { "to", toPhoneNumber },
                { "enable_msg_id", "TRUE" }
            };

            var content = new FormUrlEncodedContent(formData);
            var response = await _httpClient.PostAsync("https://sms.hollatags.com/api/send/", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Hollatags API HTTP error: {(int)response.StatusCode} - {responseBody}");
            }

            if (responseBody.StartsWith("OK:", StringComparison.OrdinalIgnoreCase))
            {
                return responseBody;
            }
            else if (responseBody.StartsWith("ERR:", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"Hollatags API error: {responseBody}");
            }
            else if (responseBody.Contains(",") && Guid.TryParse(responseBody.Split(',')[1], out _))
            {
                // Handle phone_number,uuid format
                return $"OK:{responseBody.Split(',')[1]}"; // Treat UUID as message ID
            }
            else
            {
                throw new Exception($"Unexpected Hollatags API response: {responseBody}");
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return !string.IsNullOrWhiteSpace(phoneNumber) && phoneNumber.All(c => char.IsDigit(c) || c == '+') && phoneNumber.Length >= 10;
        }
    }
}
