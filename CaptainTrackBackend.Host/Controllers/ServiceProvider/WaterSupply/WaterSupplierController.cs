using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.WaterSupply;
using CaptainTrackBackend.Application.DTO.MapModels;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers.ServiceProvider.WaterSupply
{
    [Route("api/[controller]")]
    [ApiController]
    public class WaterSupplierController : ControllerBase
    {
        private readonly IWaterSupplierService _waterSupplierService;
        public WaterSupplierController(IWaterSupplierService waterSupplierService)
        {
            _waterSupplierService = waterSupplierService;
        }
        [HttpPost("register-store-owner/{userId}")]
        public async Task<IActionResult> RegisterStoreOwner(Guid userId, [FromForm] ServiceProviderRequest request)
        {
            var files = Request.Form.Files;
            var response = await _waterSupplierService.RegisterStoreOwner(userId, request, files);
            return response.Success ? Ok(response) : BadRequest(response);
        }
        [HttpPost("register-freelancer/{userId}")]
        public async Task<IActionResult> RegisterFreelancer(Guid userId, [FromForm] FreelancerRequest request, IFormFile file)
        {
            var response = await _waterSupplierService.RegisterFreelancer(userId, request, file);
            return response.Success ? Ok(response) : BadRequest(response);
        }
        [HttpPost("set-price")]
        public async Task<IActionResult> SetPrice(decimal price, Guid? userId = null)
        {
            var response = await _waterSupplierService.SetPrice(price, userId);
            return response.Success ? Ok(response) : BadRequest(response);
        }
        [HttpGet("get-water-supplier/{userId}")]
        public async Task<IActionResult> GetWaterSupplier(Guid userId)
        {
            var response = await _waterSupplierService.GetWaterSupplier(userId);
            return response.Success ? Ok(response) : BadRequest(response);
        }
        [HttpGet("get-water-suppliers")]
        public async Task<IActionResult> GetWaterSuppliers()
        {
            var response = await _waterSupplierService.GetWaterSuppliers();
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
