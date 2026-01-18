using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.RideHailing;
using CaptainTrackBackend.Application.DTO.ServiceProviders.Ridehailing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CaptainTrackBackend.Host.Controllers.ServiceProvider.RideHailing
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly IDriversServices _driverService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

        public DriverController(IDriversServices driverService, IWebHostEnvironment webHostEnvironment)
        {
            _driverService = driverService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("register/{userId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Register([FromRoute]Guid userId, [FromForm] DriverRequestDto driverDto)
        {
            var files = HttpContext.Request.Form.Files;
            var result = await _driverService.Register(userId, driverDto, files);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("approve/{id}")]
        public async Task<IActionResult> Approve([FromRoute] Guid id)
        {
            var result = await _driverService.Approve(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var result = await _driverService.Get(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("getByUserId/{userId}")]
        public async Task<IActionResult> GetByUserId([FromRoute] Guid userId)
        {
            var result = await _driverService.GetByUserId(userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("getByEmail/{email}")]
        public async Task<IActionResult> GetByEmail([FromRoute] string email)
        {
            var result = await _driverService.Get(email);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("update/{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] DriverUpdateDto driverDto)
        {
            var result = await _driverService.Update(id, driverDto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("getAvailable")]
        public async Task<IActionResult> GetAvailable()
        {
            var result = await _driverService.GetAvailable();
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
