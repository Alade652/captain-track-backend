using CaptainTrackBackend.Application.Abstraction.Interface.Maps;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ITrackingHub _trackingHub;
        public LocationController(ITrackingHub trackingHub)
        {
            _trackingHub = trackingHub;
        }

        [HttpPost("send-location/{userId}")]
        public async Task<IActionResult> SendLocation(Guid userId, double latitude, double longitude)
        {
            try
            {
                await _trackingHub.SendLocation(userId, latitude, longitude);
                return Ok("Location sent successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error sending location: {ex.Message}");
            }
        }

        [HttpGet("get-location/{userId}")]
        public async Task<IActionResult> GetLocation(Guid userId)
        {
            try
            {
                var location = await _trackingHub.GetLocation(userId);
                if (location == null)
                {
                    return NotFound("Location not found.");
                }
                return Ok(location);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving location: {ex.Message}");
            }
        }
    }
}
