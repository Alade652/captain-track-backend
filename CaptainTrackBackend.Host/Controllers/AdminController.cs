using CaptainTrackBackend.Application.Abstraction.Interface.Services;
using CaptainTrackBackend.Application.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminServices _amdinSerVice;
        public AdminController(IAdminServices amdinSerVice)
        {
            _amdinSerVice = amdinSerVice;
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] AdminDto adminDto)
        {
            var result = await _amdinSerVice.Create(adminDto);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
