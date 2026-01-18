using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.Courier;
using CaptainTrackBackend.Application.DTO.ServiceProviders.Ambulance;
using CaptainTrackBackend.Application.DTO.ServiceProviders.Courier;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers.ServiceProvider.Courier
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourierServiceController : ControllerBase
    {
        private readonly ICourierService _service;

        public CourierServiceController(ICourierService courierService)
        {
            _service = courierService;
        }

        [HttpPost("initiate-booking/{customerId}")]
        public async Task<IActionResult> InitiateBooking(Guid customerId, CourierServiceRequest request, Guid? riderOrparkuserId)
        {
            var response = await _service.InitBooking(customerId, request, riderOrparkuserId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("book/{courierServiceId}")]
        public async Task<IActionResult> Book(Guid courierServiceId)
        {
            var response = await _service.Book(courierServiceId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("get-booking-by-riderOrparkuserIdOrcustomerId/{riderOrparkuserIdOrcustomerId}")]
        public async Task<IActionResult> GetBookingsByUserId(Guid riderOrparkuserIdOrcustomerId)
        {
            var response = await _service.GetBookings(riderOrparkuserIdOrcustomerId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("get-pending/{location}")]
        public async Task<IActionResult> GetPendingBookings(string location)
        {
            var response = await _service.GetPendings(location);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpDelete("cancel-booking/{courierServiceId}")]
        public async Task<IActionResult> CancelBooking(Guid courierServiceId)
        {
            var response = await _service.CancelBooking(courierServiceId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("accept-booking/{courierServiceId}")]
        public async Task<IActionResult> AcceptBooking(Guid courierServiceId)
        {
            var response = await _service.AcceptBooking(courierServiceId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("accept-offer/{bookingId}/{riderUserId}/{offerAmount}")]
        public async Task<IActionResult> AcceptOffer(Guid bookingId, Guid riderUserId, decimal offerAmount)
        {
            var response = await _service.AcceptOffer(bookingId, riderUserId, offerAmount);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("raise-price/{bookingId}/{newPrice}")]
        public async Task<IActionResult> RaisePrice(Guid bookingId, decimal newPrice)
        {
            var response = await _service.RaisePrice(bookingId, newPrice);
            return response.Success ? Ok(response) : BadRequest(response);

        }

    }
}
