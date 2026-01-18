using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers.ServiceProvider.DryCleaning
{
    [Route("api/[controller]")]
    [ApiController]
    public class DryCleanerItemController : ControllerBase
    {
        private readonly ILaundryItemService _itemService;
        public DryCleanerItemController(ILaundryItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpPost("addDryCleanerItem/{dryCleanerUserId}")]
        public async Task<IActionResult> AddDryCleanerItem(Guid dryCleanerUserId, Guid itemId, decimal price)
        {
            var result = await _itemService.AddDryCleanerItem(dryCleanerUserId, itemId, price);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("addItem")]
        public async Task<IActionResult> AddItem([FromForm]ItemRequestDto requestDto, IFormFile file)
        {
            var result = await _itemService.AddItem(requestDto, file);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("getItems/{dryCleanerUserid}")]
        public async Task<IActionResult> GetItems(Guid dryCleanerUserid)
        {
            var result = await _itemService.GetItems(dryCleanerUserid);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("getItems")]
        public async Task<IActionResult> GetItems()
        {
            var result = await _itemService.GetItems();
            return result.Success ? Ok(result) : BadRequest(result);
        }

    }
}
