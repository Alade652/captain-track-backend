using CaptainTrackBackend.Application.Abstraction.Interface.Services;
using CaptainTrackBackend.Application.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CustomerRequestDto customerDto)
        {
            var result = await _customerService.Register(customerDto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var result = await _customerService.Get(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("getByUserId/{userId}")]
        public async Task<IActionResult> GetByUserId([FromRoute] Guid userId)
        {
            var result = await _customerService.GetByUserId(userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("getByEmail/{email}")]
        public async Task<IActionResult> GetByEmail([FromRoute] string email)
        {
            var result = await _customerService.Get(email);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("update/{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] CustomerUpdateDto customerDto)
        {
            var result = await _customerService.Update(id, customerDto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _customerService.Delete(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
