using CaptainTrackBackend.Application.Abstraction.Interface.Maps;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapController : ControllerBase
    {
        private readonly IMapServices _service;
        private readonly ITrackingHub _trackingHub;
        public MapController(IMapServices service, ITrackingHub trackingHub)
        {
            _service = service;
            _trackingHub = trackingHub;
        }

        [HttpGet("getDistance")]
        public async Task<IActionResult> GetDistance(string origin, string destination)
        {
            if (string.IsNullOrWhiteSpace(origin) || string.IsNullOrWhiteSpace(destination))
                return BadRequest("Origin and destination are required.");

            var result = await _service.GetDistanceAsync(origin, destination);
            return Ok(result);
        }

        [HttpGet("getCordinates")]
        public async Task<IActionResult> GetCoordinates(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return BadRequest("AddressorLocation is required.");

            var result = await _service.GetCoordinatesAsync(address);

            var location = result.results.FirstOrDefault()?.geometry?.location;

            if (location != null)
            {
                return Ok(new
                {
                    Latitude = location.lat,
                    Longitude = location.lng,
                    Address = result.results.FirstOrDefault()?.formatted_address
                });
            }

            return BadRequest("No location data found.");
        }

        [HttpGet("getAddress")]
        public async Task<IActionResult> GetAddress(double lat, double lng)
        {
            var result = await _service.GetAddressAsync(lat, lng);
            var address = result.results.FirstOrDefault()?.formatted_address;

            if (address != null)
                return Ok(new { Address = address });

            return BadRequest("No address found.");
        }

        [HttpGet("getTimeZone")]
        public async Task<IActionResult> GetTimeZone(double lat, double lng)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var result = await _service.GetTimeZoneAsync(lat, lng, timestamp);

            if (result != null && result.status == "OK")
            {
                return Ok(new
                {
                    result.timeZoneId,
                    result.timeZoneName,
                    result.rawOffset,
                    result.dstOffset
                });
            }

            return BadRequest("Failed to fetch time zone data.");
        }

        [HttpPost("sendLocation/{userId}")]
        public async Task<IActionResult> SendLocation(Guid userId, double lat, double lng)
        {
            var result = await _trackingHub.SendLocation(userId, lat, lng);
            return Ok(result);
        }
    }
}
