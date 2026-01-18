using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.WaterSupply;
using CaptainTrackBackend.Application.DTO.ServiceProviders.WaterSupply;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers.ServiceProvider.WaterSupply
{
    [Route("api/[controller]")]
    [ApiController]
    public class WaterSupplingController : ControllerBase
    {
        private readonly IWaterSupplingService _waterSupplingService;

        public WaterSupplingController(IWaterSupplingService waterSupplingService)
        {
            _waterSupplingService = waterSupplingService;
        }

        [HttpPost("init-booking/{customerId}")]
        public async Task<IActionResult> InitBooking(Guid customerId, [FromBody] WaterSupplingRequest request, Guid? waterSupplierUserId = null)
        {
            var response = await _waterSupplingService.InitBooking(request, customerId, waterSupplierUserId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("book/{waterSupplingId}")]
        public async Task<IActionResult> Book(Guid waterSupplingId)
        {
            var response = await _waterSupplingService.Book(waterSupplingId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("get-bookings/{userId}")]
        public async Task<IActionResult> GetBookings(Guid userId)
        {
            var response = await _waterSupplingService.GetBookings(userId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("get-pending")]
        public async Task<IActionResult> GetPending(string location)
        {
            var response = await _waterSupplingService.GetPending(location);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("get-booking/{waterSupplingId}")]
        public async Task<IActionResult> GetBookingById(Guid waterSupplingId)
        {
            var response = await _waterSupplingService.GetBookingById(waterSupplingId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("cancel-booking/{waterSupplingId}")]
        public async Task<IActionResult> CancelBooking(Guid waterSupplingId)
        {
            var response = await _waterSupplingService.CancelBooking(waterSupplingId);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
