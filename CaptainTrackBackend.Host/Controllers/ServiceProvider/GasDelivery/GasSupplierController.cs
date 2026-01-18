using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.GasDelivery;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.GasDelivery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers.ServiceProvider.GasDelivery
{
    [Route("api/[controller]")]
    [ApiController]
    public class GasSupplierController : ControllerBase
    {
        private readonly IGasSupplierService _gasSupplierService;
        public GasSupplierController(IGasSupplierService gasSupplierService)
        {
            _gasSupplierService = gasSupplierService;
        }

        [HttpPost("set-price-per-kg")]
        public async Task<IActionResult> SetPricePerKg( decimal price)
        {
            var result = await _gasSupplierService.SetPricePerKg(price);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("register/{userId}")]
        public async Task<IActionResult> Register(Guid userId, [FromForm] ServiceProviderRequest request)
        {
            var files = HttpContext.Request.Form.Files;
            var result = await _gasSupplierService.Register(userId, request, files);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("register-freelancer/{userId}")]
        public async Task<IActionResult> RegisterFreelancer(Guid userId, [FromForm] FreelancerRequest request, IFormFile file )
        {
            var result = await _gasSupplierService.RegisterFreelancer(userId, request, file);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-by-user/{userId}")]
        public async Task<IActionResult> GetGasSupplierByUserId(Guid userId)
        {
            var result = await _gasSupplierService.GetGasSupplierByUserId(userId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllGasSuppliers()
        {
            var result = await _gasSupplierService.GetAllGasSuppliers();
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost("set-price/{gasSupplierUserId}")]
        public async Task<IActionResult> SetPrice(decimal pricePerKg, Guid gasSupplierUserId)
        {
            var result = await _gasSupplierService.SetPrice(pricePerKg, gasSupplierUserId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

/*        [HttpPost("update/{gasSupplierUserId}")]
        public async Task<IActionResult> Update(Guid gasSupplierUserId)
        {
            var result = await _gasSupplierService.Update(gasSupplierUserId);
            return result.Success ? Ok(result) : BadRequest(result);
        }*/

    }
}
