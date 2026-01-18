using CaptainTrackBackend.Application.Abstraction.Interface.Services;
using CaptainTrackBackend.Application.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace CaptainTrackBackend.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IFlutterwaveService _flutterwaveService;

        public PaymentController( IFlutterwaveService flutterwaveService)
        {
            _flutterwaveService = flutterwaveService;
        }

        [HttpPost("initiate/{userId}")]
        public async Task<IActionResult> InitiatePayment(Guid userId, PaymentRequest request)
        {
            var response = await _flutterwaveService.InitiatePayment(userId, request);
            return Ok(new { response.paymentLink, response.txId });
        }


        [HttpGet("callback")]
        public async Task<IActionResult> PaymentCallback(/*[FromQuery] string status,*/[FromQuery] string transaction_id)
        {
/*            if (status != "successful")
            {
                // Handle failure or cancellation
                return BadRequest("Payment was not successful.");
            }*/

            // Call verify endpoint to confirm
            var verificationResult = await _flutterwaveService.VerifyPaymentAsync(transaction_id);

            string paymentStatus = verificationResult.data.status;

            if (paymentStatus == "successful")
            {
                return Ok("Payment verified successfully!");
            }
            else
            {
                return BadRequest("Payment verification failed.");
            }
        }


    }
}
