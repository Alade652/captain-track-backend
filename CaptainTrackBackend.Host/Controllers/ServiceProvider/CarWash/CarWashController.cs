using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.CarWash;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.CarWash;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers.ServiceProvider.CarWash
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarWashController : ControllerBase
    {
        private readonly ICarWasherService _carWasherService;
        public CarWashController(ICarWasherService carWasherService)
        {
            _carWasherService = carWasherService;
        }

        [HttpPost("register/{userId}")]
        public async Task<IActionResult> Register(Guid userId, [FromForm] ServiceProviderRequest request)
        {
            var files = Request.Form.Files;
            var response = await _carWasherService.RegisterStoreowner(userId, request, files);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("register-freelancer/{userId}")]
        public async Task<IActionResult> RegisterFreelancer(Guid userId, [FromForm] FreelancerRequest request, IFormFile file)
        {
            var response = await _carWasherService.RegisterFreelancer(userId, request, file);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("get-carwash/{userId}")]
        public async Task<IActionResult> GetCarWash(Guid userId)
        {
            var response = await _carWasherService.GetCarWash(userId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("get-carwashes")]
        public async Task<IActionResult> GetCarwashes()
        {
            var response = await _carWasherService.GetCarwashes();
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost("add-carwash-item")]
        public async Task<IActionResult> AddCarWashItem([FromBody]CarWashItemRequest request, Guid? userId = null)
        {
            var response = await _carWasherService.AddCarWashItem(request, userId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("get-carwash-items")]
        public async Task<IActionResult> GetCarWashItems(Guid? userId = null)
        {
            var response = await _carWasherService.GetCarWashItems(userId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPatch("update-carwash-item/{itemId}")]
        public async Task<IActionResult> UpdateCarWashItem(Guid itemId, [FromBody] CarWashItemRequest request)
        {
            var response = await _carWasherService.UpdateCarWashItem(itemId, request);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
