using CaptainTrackBackend.Application.Abstraction.Interface.Services;
using CaptainTrackBackend.Application.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingService _ratingsService;

        public RatingsController(IRatingService ratingsService)
        {
            _ratingsService = ratingsService;
        }

        [HttpPost("create/{createdBy}")]
        public async Task<IActionResult> Create(Guid createdBy, RatingRequest request)
        {
            var result = await _ratingsService.CreateRating(createdBy, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("getCustomerRatings/{customerId}")]
        public async Task<IActionResult> GetCustomerRatings([FromRoute] Guid customerId)
        {
            var result = await _ratingsService.GetCustomerRatings(customerId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("getVendorRatings/{vendorUserId}")]
        public async Task<IActionResult> GetVendorRatings([FromRoute] Guid vendorUserId)
        {
            var result = await _ratingsService.GetVendorRatings(vendorUserId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

/*        [HttpGet("getRatings/{customerId}/{vendorId}")]
        public async Task<IActionResult> GetRatings([FromRoute] Guid customerId, [FromRoute] Guid vendorId)
        {
            var result = await _ratingsService.GetRatings(customerId, vendorId);
            return result.Success ? Ok(result) : BadRequest(result);
        }*/

    }
}
