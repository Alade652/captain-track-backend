using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.HouseCleaning;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.HouseCleaning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers.ServiceProvider.HouseCleaning
{
    [Route("api/[controller]")]
    [ApiController]
    public class HouseCleaningController : ControllerBase
    {
        private readonly IHouseCleaningService _houseCleaningService;
        public HouseCleaningController(IHouseCleaningService houseCleaningService)
        {
            _houseCleaningService = houseCleaningService;
        }

        [HttpPost("InitBooking/{customerId}")]
        public async Task<IActionResult> InitBooking(Guid customerId, [FromBody] HouseCleaningRequest request, Guid? houseCleanerUserId = null)
        {
            var response = await _houseCleaningService.InitBooking(customerId, request, houseCleanerUserId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("Book/{houseCleaningId}")]
        public async Task<IActionResult> Book(Guid houseCleaningId)
        {
            var response = await _houseCleaningService.Book(houseCleaningId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("GetHouseCleanings/{userId}")]
        public async Task<IActionResult> GetHouseCleanings(Guid userId)
        {
            var response = await _houseCleaningService.GetHouseCleanings(userId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("GetPending/{location}")]
        public async Task<IActionResult> GetPending(string location)
        {
            var response = await _houseCleaningService.GetPending(location);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost("AcceptBooking/{houseCleaningId}")]
        public async Task<IActionResult> AcceptBooking(Guid houseCleaningId)
        {
            var response = await _houseCleaningService.AcceptBooking(houseCleaningId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("CancelBooking/{houseCleaningId}")]
        public async Task<IActionResult> CancelBooking(Guid houseCleaningId)
        {
            var response = await _houseCleaningService.CancelBooking(houseCleaningId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("AcceptOffer/{houseCleaningId}")]
        public async Task<IActionResult> AcceptOffer(Guid houseCleaningId, Guid userId, decimal offerAmount)
        {
            var response = await _houseCleaningService.AcceptOffer(houseCleaningId, userId, offerAmount);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPatch("raise-fare/{houseCleaningId}")]
        public async Task<IActionResult> RaiseFare(Guid houseCleaningId, decimal amount)
        {
            var response = await _houseCleaningService.RaisePrice(houseCleaningId, amount);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
