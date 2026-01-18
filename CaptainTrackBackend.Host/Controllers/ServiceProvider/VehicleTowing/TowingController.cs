using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.VehicleTowing;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.VehicleTowing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers.ServiceProvider.VehicleTowing
{
    [Route("api/[controller]")]
    [ApiController]
    public class TowingController : ControllerBase
    {
        private readonly ITowingService _towingService;
        public TowingController(ITowingService towingService)
        {
            _towingService = towingService;
        }

        [HttpPost("init-Booking")]
        public async Task<IActionResult> InitBooking(Guid customerId, TowingRequest request, Guid? truckOperatoruserId = null)
        {
            var response = await _towingService.InitBooking(customerId, request, truckOperatoruserId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("Book/{towingId}")]
        public async Task<IActionResult> Book(Guid towingId)
        {
            var response = await _towingService.Book(towingId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("get-towings/{userid}")]
        public async Task<IActionResult> GetTowings(Guid userid)
        {
            var response = await _towingService.GetBookings(userid);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("get-pending/{location}")]
        public async Task<IActionResult> GetPendingBookings(string location)
        {
            var response = await _towingService.GetPendings(location);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost("accept-booking/{towingId}")]
        public async Task<IActionResult> AcceptBooking(Guid towingId)
        {
            var response = await _towingService.AcceptBooking(towingId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("cancel-booking/{towingId}")]
        public async Task<IActionResult> CancelBooking(Guid towingId)
        {
            var response = await _towingService.CancelBooking(towingId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("reject-booking/{towingId}")]
        public async Task<IActionResult> RejectBooking(Guid towingId)
        {
            var response = await _towingService.RejectBooking(towingId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("accept-offer/{towingId}")]
        public async Task<IActionResult> AcceptOffer(Guid towingId, Guid userId, decimal offerAmount)
        {
            var response = await _towingService.AcceptOffer(towingId, userId, offerAmount);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPatch("raise-fare/")]
        public async Task<IActionResult> RaiseFare(Guid towingId, decimal amount)
        {
            var response = await _towingService.RaisePrice(towingId, amount);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
