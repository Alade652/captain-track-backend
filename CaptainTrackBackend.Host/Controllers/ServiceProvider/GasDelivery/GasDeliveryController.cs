using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.GasDelivery;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.GasDelivery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers.ServiceProvider.GasDelivery
{
    [Route("api/[controller]")]
    [ApiController]
    public class GasDeliveryController : ControllerBase
    {
        private readonly IGasDeliveryService _gasDeliveryService;
        public GasDeliveryController(IGasDeliveryService gasDeliveryService)
        {
            _gasDeliveryService = gasDeliveryService;
        }

        [HttpPost("init-booking/{customerId}/{gasSupplierUserId}")]
        public async Task<IActionResult> InitBooking(Guid customerId, Guid gasSupplierUserId, [FromBody] GasDeliveryRequestDto request)
        {
            var result = await _gasDeliveryService.InitBooking(customerId, gasSupplierUserId, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("init-booking-freelancer/{customerId}")]
        public async Task<IActionResult> InitBooking(Guid customerId, [FromBody] GasDeliveryRequestDto request)
        {
            var result = await _gasDeliveryService.InitBooking(customerId, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("book/{gasDeliveryId}")]
        public async Task<IActionResult> Book(Guid gasDeliveryId)
        {
            var result = await _gasDeliveryService.Book(gasDeliveryId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-pending/{location}")]
        public async Task<IActionResult> GetPendingDeliveries(string location)
        {
            var result = await _gasDeliveryService.GetPending(location);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("getBookings/{userId}")]
        public async Task<IActionResult> GetBookings(Guid userId)
        {
            var result = await _gasDeliveryService.GetBookings(userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("accept-booking/{gasDeliveryId}")]
        public async Task<IActionResult> AcceptBooking(Guid gasDeliveryId)
        {
            var result = await _gasDeliveryService.AcceptBooking(gasDeliveryId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("cancel-booking/{gasDeliveryId}")]
        public async Task<IActionResult> CancelBooking(Guid gasDeliveryId)
        {
            var result = await _gasDeliveryService.CancelBooking(gasDeliveryId);
            return result.Success ? Ok(result) : BadRequest(result);

        }

        [HttpPost("reject-booking/{gasDeliveryId}")]
        public async Task<IActionResult> RejectBooking(Guid gasDeliveryId)
        {
            var result = await _gasDeliveryService.RejectBooking(gasDeliveryId);
            return result.Success ? Ok(result) : BadRequest(result);

        }

        [HttpPost("accept-offer/{gasDeliveryId}")]
        public async Task<IActionResult> AcceptOffer(Guid gasDeliveryId, Guid gasSupplierUserid, decimal offerAmount)
        {
            var result = await _gasDeliveryService.AcceptOffer(gasDeliveryId, gasSupplierUserid, offerAmount);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("raise-fare/{gasDeliveryId}")]
        public async Task<IActionResult> RaiseFare(Guid gasDeliveryId, decimal amount)
        {
            var response = await _gasDeliveryService.RaisePrice(gasDeliveryId, amount);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
