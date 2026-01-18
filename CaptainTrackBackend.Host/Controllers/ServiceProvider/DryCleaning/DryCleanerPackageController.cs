using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers.ServiceProvider.DryCleaning
{
    [Route("api/[controller]")]
    [ApiController]
    public class DryCleanerPackageController : ControllerBase
    {
        private readonly ILaundryPackageService _packageService;
        public DryCleanerPackageController(ILaundryPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpPost("add/{dryCleanerUserid}")]
        public async Task<IActionResult> Add(Guid dryCleanerUserid, [FromForm] PackageRequestDto request, IFormFile file)
        {
            var result = await _packageService.Add(dryCleanerUserid, request, file);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromForm] PackageRequestDto request, IFormFile file)
        {
            var result = await _packageService.Add(request, file);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("getPackages/{dryCleanerUserid}")]
        public async Task<IActionResult> GetPackages(Guid dryCleanerUserid)
        {
            var result = await _packageService.GetPackages(dryCleanerUserid);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("getPackages")]
        public async Task<IActionResult> GetPackages()
        {
            var result = await _packageService.GetPackages();
            return result.Success ? Ok(result) : BadRequest(result);
        }

    }
}
