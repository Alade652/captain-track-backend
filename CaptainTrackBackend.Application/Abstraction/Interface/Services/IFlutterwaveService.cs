using CaptainTrackBackend.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services
{
    public interface IFlutterwaveService
    {
        Task<(string paymentLink, string txId)> InitiatePayment(Guid UserId, PaymentRequest request);
        Task<dynamic> VerifyPaymentAsync(string txId);
    }
}
