using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.DryCleaning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers.ServiceProvider.DryCleaning
{
    [Route("api/[controller]")]
    [ApiController]
    public class DryCleanerController : ControllerBase
    {
        private readonly IDryCleanerService _dryCleanerService;

        public DryCleanerController(IDryCleanerService dryCleanerService)
        {
            _dryCleanerService = dryCleanerService;
        }

        [HttpPost("registerStoreOwner/{userId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Register([FromRoute] Guid userId, [FromForm] ServiceProviderRequest request)
        {
            var files = HttpContext.Request.Form.Files;
            var result = await _dryCleanerService.RegisterStoreOwner(userId, request, files);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("registerFreelancer/{userId}")]
        public async Task<IActionResult> RegisterFreelancer([FromRoute] Guid userId, [FromForm] FreelancerRequest request, IFormFile file)
        {
            var result = await _dryCleanerService.RegisterFreelancer(userId, request, file);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _dryCleanerService.GetAll();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _dryCleanerService.Get(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("getByUserId/{userId}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var result = await _dryCleanerService.GetByUserId(userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
