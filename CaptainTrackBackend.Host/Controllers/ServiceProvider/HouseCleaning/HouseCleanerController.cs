using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.HouseCleaning;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.HouseCleaning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers.ServiceProvider.HouseCleaning
{
    [Route("api/[controller]")]
    [ApiController]
    public class HouseCleanerController : ControllerBase
    {
        private readonly IHouseCleanerService _houseCleanerService;
        public HouseCleanerController(IHouseCleanerService houseCleanerService)
        {
            _houseCleanerService = houseCleanerService;
        }

        [HttpPost("register-store-owner")]
        public async Task<IActionResult> RegisterStoreOwner(Guid userId, [FromForm] ServiceProviderRequest request)
        {
            var files = Request.Form.Files;
            var response = await _houseCleanerService.RegisterStoreOwner(userId, request, files);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("register-freelancer")]
        public async Task<IActionResult> RegisterFreelancer(Guid userId, [FromForm] FreelancerRequest request, IFormFile file)
        {
            var response = await _houseCleanerService.RegisterFreelancer(userId, request, file);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("get-houseCleaner/{userId}")]
        public async Task<IActionResult> GetHouseCleaner(Guid userId)
        {
            var response = await _houseCleanerService.GetHouseCleaner(userId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("get-house-cleaners")]
        public async Task<IActionResult> GetHouseCleaners()
        {
            var response = await _houseCleanerService.GetHouseCleaners();
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost("add-package")]
        public async Task<IActionResult> AddPackage([FromForm] HouseCleanerPackageRequest request, Guid? houseCleanerUserId = null)
        {
            var response = await _houseCleanerService.AddPackage(request, houseCleanerUserId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("get-packages")]
        public async Task<IActionResult> GetPackages(Guid? houseCleanerUserId)
        {
            var response = await _houseCleanerService.GetPackages(houseCleanerUserId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost("add-item/{houseCleanerPackageId}")]
        public async Task<IActionResult> AddItem(Guid houseCleanerPackageId, [FromBody] HouseCleanerItemRequest request)
        {
            var response = await _houseCleanerService.AddItem(houseCleanerPackageId, request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("get-items/{houseCleanerPackageId}")]
        public async Task<IActionResult> GetItems(Guid houseCleanerPackageId)
        {
            var response = await _houseCleanerService.GetItems(houseCleanerPackageId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPatch("update-item-price/{houseCleanerItemId}")]
        public async Task<IActionResult> UpdateItemPrice(Guid houseCleanerItemId, [FromBody] decimal price)
        {
            var response = await _houseCleanerService.UpdateItemPrice(houseCleanerItemId, price);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
