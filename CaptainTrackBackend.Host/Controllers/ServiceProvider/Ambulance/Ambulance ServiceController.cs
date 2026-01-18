using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.Ambulance;
using CaptainTrackBackend.Application.DTO.ServiceProviders.Ambulance;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers.ServiceProvider.Ambulance
{
    [Route("api/[controller]")]
    [ApiController]
    public class Ambulance_ServiceController : ControllerBase
    {
        private readonly IAmbulanceBookingService _service;
        public Ambulance_ServiceController(IAmbulanceBookingService service)
        {
            _service = service;
        }

        [HttpPost("initiate-booking/{customerId}")]
        public async Task<IActionResult> InitiateBooking(Guid customerId, AmbulanceBookingRequest request, Guid? ambulanceuserId = null)
        {
            var response = await _service.InitiateAmbulanceBookingAsync(customerId, request, ambulanceuserId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("book/{ambulanceBookingId}")]
        public async Task<IActionResult> Book(Guid ambulanceBookingId)
        {
            var response = await _service.Book(ambulanceBookingId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("get-booking-by-userId/{userId}")]
        public async Task<IActionResult> GetBookingsByUserId(Guid userId)
        {
            var response = await _service.GetBookingsByUserIdAsync(userId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("get-pending/{location}")]
        public async Task<IActionResult> GetPendingBookings(string location)
        {
            var response = await _service.GetPendings(location);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost("accept-booking/{bookingId}")]
        public async Task<IActionResult> AcceptBooking(Guid bookingId)
        {
            var response = await _service.AcceptBookingAsync(bookingId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("cancel-booking/{bookingId}")]
        public async Task<IActionResult> CancelBooking(Guid bookingId)
        {
            var response = await _service.CancelBookingAsync(bookingId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

 /*       [HttpPost("reject-booking/")]
        public async Task<IActionResult> RejectBooking(Guid bookingId)
        {
            var response = await _service.Re(negotiationId);
            return response.Success ? Ok(response) : BadRequest(response);
        }*/

        [HttpPost("accept-offer/{ambulanceBookingId}")]
        public async Task<IActionResult> AcceptOffer(Guid ambulanceBookingId, Guid ambulanceUserId, decimal offerAmount)
        {
            var response = await _service.AcceptOffer(ambulanceBookingId, ambulanceUserId, offerAmount);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPatch("raise-fare/{bookingId}")]
        public async Task<IActionResult> RaiseFare(Guid bookingId, decimal amount)
        {
            var response = await _service.RaisePrice(bookingId, amount);
            return response.Success ? Ok(response) : BadRequest(response);
        }


    }
}
