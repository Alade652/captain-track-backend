using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.Courier;
using CaptainTrackBackend.Application.DTO.MapModels;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.Courier;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers.ServiceProvider.Courier
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourierController : ControllerBase
    {
        private readonly IRiderorParkService _rideorParkService;

        public CourierController(IRiderorParkService rideorParkService)
        {
            _rideorParkService = rideorParkService;
        }

        [HttpPost("register-park/{userId}")]
        public async Task<IActionResult> RegisterPark(Guid userId, [FromForm] ServiceProviderRequest request)
        {
            var files = Request.Form.Files;
            var response = await _rideorParkService.RegisterPark(userId, request, files);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("register-rider/{userId}")]
        public async Task<IActionResult> RegisterRider(Guid userId, [FromForm] FreelancerRequest request, IFormFile file)
        {
            var response = await _rideorParkService.RegisterRider(userId, request, file);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("get-parks")]
        public async Task<IActionResult> GetParks()
        {
            var response = await _rideorParkService.GetParks();
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("get-park/{userId}")]
        public async Task<IActionResult> GetPark(Guid userId)
        {
            var response = await _rideorParkService.GetPark(userId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("get-rider/{userId}")]
        public async Task<IActionResult> GetRider(Guid userId)
        {
            var response = await _rideorParkService.GetRider(userId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("add-item")]
        public async Task<IActionResult> AddItem(CourierVehicleRequest request, Guid? userId = null)
        {
            var response = await _rideorParkService.AddItem(request, userId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("get-items")]
        public async Task<IActionResult> GetItems(Guid? userId = null)
        {
            var response = await _rideorParkService.GetItems(userId);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
