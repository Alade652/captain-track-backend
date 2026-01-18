using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.Ambulance;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers.ServiceProvider.Ambulance
{
    [Route("api/[controller]")]
    [ApiController]
    public class AmbulanceController : ControllerBase
    {
        private readonly IAmbulanceSerivice _ambulanceService;
        public AmbulanceController(IAmbulanceSerivice ambulanceService)
        {
            _ambulanceService = ambulanceService;
        }

        [HttpPost("RegisterStoreOwner/{userId}")]
        public async Task<IActionResult> RegisterStoreOwner(Guid userId, [FromForm] ServiceProviderRequest request)
        {
            var files = Request.Form.Files;
            var response = await _ambulanceService.RegisterStoreOwner(userId, request, files);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("RegisterFreelancer/{userId}")]
        public async Task<IActionResult> RegisterFreelancer(Guid userId, [FromForm] FreelancerRequest request, IFormFile file)
        {
            var response = await _ambulanceService.RegisterFreelancer(userId, request, file);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("GetAllAmbulances")]
        public async Task<IActionResult> GetAllAmbulances()
        {
            var response = await _ambulanceService.GetAllAmbulancesAsync();
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("GetAmbulanceByUserId/{userId}")]
        public async Task<IActionResult> GetAmbulanceByUserId(Guid userId)
        {
            var response = await _ambulanceService.GetAmbulanceByUserIdAsync(userId);
            return response.Success ? Ok(response) : NotFound(response);
        }


        [HttpPost("SetPrice")]
        public async Task<IActionResult> SetPrice(decimal price, Guid? ambulanceuserId = null)
        {
            var response = await _ambulanceService.SetPrice(price, ambulanceuserId);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
