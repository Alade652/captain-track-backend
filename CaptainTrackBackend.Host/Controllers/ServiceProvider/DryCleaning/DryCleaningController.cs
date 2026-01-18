using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.DryCleaning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers.ServiceProvider.DryCleaning
{
    [Route("api/[controller]")]
    [ApiController]
    public class DryCleaningController : ControllerBase
    {
        private readonly IDryCleaningService _dryCleaningService;
        public DryCleaningController(IDryCleaningService dryCleaningService)
        {
            _dryCleaningService = dryCleaningService;
        }

        [HttpPost("initBook/{customerId}/{dryCleanerUserid}")]
        public async Task<IActionResult> InitBook(Guid customerId, Guid dryCleanerUserid, [FromBody] DryCleaningRequestDto dryCleaningRequest)
        {
            var result = await _dryCleaningService.InitBook(customerId, dryCleanerUserid, dryCleaningRequest);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("initBook/{customerId}")]
        public async Task<IActionResult> InitBook(Guid customerId, [FromBody] DryCleaningRequestDto dryCleaningRequest)
        {
            var result = await _dryCleaningService.InitBookFreelancer(customerId, dryCleaningRequest);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("getPending/{location}")]
        public async Task<IActionResult> GetPending(string location)
        {
            var result = await _dryCleaningService.GetPending(location);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("getBookings/{userId}")]
        public async Task<IActionResult> GetBookings(Guid userId)
        {
            var result = await _dryCleaningService.GetBookings(userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("book/{dryCleaningId}")]
        public async Task<IActionResult> Book(Guid dryCleaningId)
        {
            var result = await _dryCleaningService.Book(dryCleaningId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("accept-booking/{dryCleaningId}")]
        public async Task<IActionResult> AcceptBooking(Guid dryCleaningId)
        {
            var result = await _dryCleaningService.AccepBooking(dryCleaningId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("cancel-booking/{dryCleaningId}")]
        public async Task<IActionResult> CancelBooking(Guid dryCleaningId)
        {
            var result = await _dryCleaningService.CancelBooking(dryCleaningId);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpPost("accept-Offer/{dryCleaningId}")]
        public async Task<IActionResult> AcceptOffer(Guid dryCleaningId, Guid dryCleanerUserId, decimal amount)
        {
            var result = await _dryCleaningService.AcceptOffer(dryCleaningId, dryCleanerUserId, amount);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("raise-fare/{dryCleaningId}")]
        public async Task<IActionResult> RaiseFare(Guid dryCleaningId, decimal amount)
        {
            var response = await _dryCleaningService.RaisePrice(dryCleaningId, amount);
            return response.Success ? Ok(response) : BadRequest(response);
        }


    }
}
