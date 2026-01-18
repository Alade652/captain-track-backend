using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Domain.Entities;
using CloudinaryDotNet;
using Flutterwave.Net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Persistence.PaymentIntegration
{
    public class FlutterwaveService : IFlutterwaveService
    {
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;
        private readonly string _baseUrl;
        private readonly IUnitofWork _unitOfWork;

        public FlutterwaveService(IConfiguration configuration, IUnitofWork unitofWork)
        {
            _httpClient = new HttpClient();
            _secretKey = configuration["Flutterwave:SecretKey"];
            _baseUrl = configuration["Flutterwave:BaseUrl"];
            _unitOfWork = unitofWork;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _secretKey);
        }

        public async Task<(string paymentLink, string txId)> InitiatePayment(Guid userId, PaymentRequest request)
        {
            var user = await _unitOfWork.User.GetAsync(userId);
            var payload = new
            {
                tx_ref = $"TX-{Guid.NewGuid().ToString("N")}",
                amount = request.amount,
                currency = "NGN",
                redirect_url = request.redirectUrl,
                customer = new
                {
                    email = user.Email,
                    name = user.FullName
                },
                customizations = new
                {
                    title = request.paymentTitle,
                    description = request.paymentDescription,
                }
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/payments", content);

            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error initiating payment: {responseString}");
            }

            var result = JsonConvert.DeserializeObject<dynamic>(responseString);

           /* var payment = new Payment
            {
                UserId = userId,
                Amount = payload.amount,
                Currency = "NGN",
                Tx_ref = result.data.tx_ref,
                PaymentLink = result.data.link,
                TransactionId = result.data.id,
                Redirect_url = payload.redirect_url,
                Title = payload.customizations.title,
                Description = payload.customizations.description,
                Status = "pending",
                CreatedOn = DateTime.UtcNow,
                CreatedBy = userId
            };

            await _unitOfWork.Payment.AddAsync(payment);

           */ 
            string paymentLink = result.data.link;
            string txId = result.data.id;

            return (paymentLink, txId);

        }

        public async Task<dynamic> VerifyPaymentAsync(string txId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/transactions/{txId}/verify");
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error verifying payment: {responseString}");
            }
            var result = JsonConvert.DeserializeObject<dynamic>(responseString);
            if (result.data.status != "successful")
            {
                throw new Exception("Payment verification failed.");
            }
           /* var payment = await _unitOfWork.Payment.GetAsync(x => x.TransactionId == txId);
            await _unitOfWork.Payment.UpdateAsync(payment);*/
            return result;
        }

    }
}
