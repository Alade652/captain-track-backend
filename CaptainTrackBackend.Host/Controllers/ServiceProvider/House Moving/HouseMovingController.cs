using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.House_Moving;
using CaptainTrackBackend.Application.DTO.ServiceProviders.House_Moving;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers.ServiceProvider.House_Moving
{
    [Route("api/[controller]")]
    [ApiController]
    public class HouseMovingController : ControllerBase
    {
        private readonly IHouseMovingService _houseMovingService;

        public HouseMovingController (IHouseMovingService houseMovingService)
        {
            _houseMovingService = houseMovingService;
        }

        [HttpPost("initiate-booking/{customerId}")]
        public async Task<IActionResult> InitiateBooking(Guid customerId,HouseMovingRequest request, Guid? houseMoverUserId = null)
        {
            var response = await _houseMovingService.InitiateBooking(customerId, request, houseMoverUserId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("book/{houseMovingId}")]
        public async Task<IActionResult> Book(Guid houseMovingId)
        {
            var response = await _houseMovingService.Book(houseMovingId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("get-pendings/{location}")]
        public async Task<IActionResult> GetPendings(string location)
        {
            var response = await _houseMovingService.GetPending(location);
            return response.Success ? Ok(response) : NotFound(response);
        }


        [HttpGet("get-houseMovings/{userId}")]
        public async Task<IActionResult> GetHouseMovings(Guid userId)
        {
            var response = await _houseMovingService.GetBookings(userId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost("accept-booking/{houseMovingId}")]
        public async Task<IActionResult> AcceptPrice(Guid houseMovingId)
        {
            var response = await _houseMovingService.AcceptBooking(houseMovingId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("cancel-booking/{houseMovingId}")]
        public async Task<IActionResult> CancelBooking(Guid houseMovingId)
        {
            var response = await _houseMovingService.CancelBooking(houseMovingId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("cancel-offer/{houseMovingId}")]
        public async Task<IActionResult> RejectBooking(Guid houseMovingId)
        {
            var response = await _houseMovingService.RejectBooking(houseMovingId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("accept-offer/{houseMovingId}")]
        public async Task<IActionResult> AcceptOffer(Guid houseMovingId, Guid userId, decimal offerAmount)
        {
            var response = await _houseMovingService.AcceptOffer(houseMovingId, userId, offerAmount);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPatch("raise-fare/{houseMovingId}")]
        public async Task<IActionResult> RaiseFare(Guid houseMovingId, decimal amount)
        {
            var response = await _houseMovingService.RaisePrice(houseMovingId, amount);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
