using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.VehicleTowing;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.VehicleTowing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers.ServiceProvider.VehicleTowing
{
    [Route("api/[controller]")]
    [ApiController]
    public class TruckOperatorController : ControllerBase
    {
        private readonly ITruckOperatorService _truckOperatorService;
        public TruckOperatorController(ITruckOperatorService truckOperatorService)
        {
            _truckOperatorService = truckOperatorService;
        }

        [HttpPost("register-store-owner")]
        public async Task<IActionResult> RegisterStoreOwner(Guid userId, [FromForm] ServiceProviderRequest request)
        {
            var files = Request.Form.Files;
            var response = await _truckOperatorService.RegisterStoreOwner(userId, request, files);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("register-freelancer")]
        public async Task<IActionResult> RegisterFreelancer(Guid userId, [FromForm] FreelancerRequest request, IFormFile file)
        {
            var response = await _truckOperatorService.RegisterFreelancer(userId, request, file);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("get-truck-operators")]
        public async Task<IActionResult> GetTruckOperators()
        {
            var response = await _truckOperatorService.GetAll();
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost("add-truck")]
        public async Task<IActionResult> AddTruck([FromForm] TruckRequest request, Guid? truckOperatorUserId = null)
        {
            var response = await _truckOperatorService.AddTruck(request, truckOperatorUserId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("get-trucks")]
        public async Task<IActionResult> GetTrucks(Guid? truckOperatorUserId)
        {
            var response = await _truckOperatorService.GetTrucks(truckOperatorUserId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("get-by-user/{userId}")]
        public async Task<IActionResult> GetTruckOperatorByUserId(Guid userId)
        {
            var response = await _truckOperatorService.GetByUserId(userId);
            return response.Success ? Ok(response) : NotFound(response);
        }
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetTruckOperatorById(Guid id)
        {
            var response = await _truckOperatorService.Get(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

    }
}
