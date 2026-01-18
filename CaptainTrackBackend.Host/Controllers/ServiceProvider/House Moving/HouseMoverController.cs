using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.House_Moving;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.House_Moving;
using CaptainTrackBackend.Domain.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities;

namespace CaptainTrackBackend.Host.Controllers.ServiceProvider.House_Moving
{
    [Route("api/[controller]")]
    [ApiController]
    public class HouseMoverController : ControllerBase
    {
        private readonly IHouseMoverService _houseMoverService;
        public HouseMoverController(IHouseMoverService houseMoverService)
        {
            _houseMoverService = houseMoverService;
        }

        [HttpPost("register-storeOwner/{userId}")]
        public async Task<IActionResult> RegisterStoreOwner(Guid userId,[FromForm] ServiceProviderRequest request)
        {
            var files = Request.Form.Files;
            var response = await _houseMoverService.RegisterStoreOwner(userId, request, files);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("register-freelancer/{userId}")]
        public async Task<IActionResult> RegisterFreeLancer(Guid userId,[FromForm] FreelancerRequest request, IFormFile file)
        {
            var response = await _houseMoverService.RegisterFreeLancer(userId, request, file);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("get-houseMover/{userId}")]
        public async Task<IActionResult> GetHouseMover(Guid userId)
        {
            var response = await _houseMoverService.GetHouseMover(userId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("get-houseMovers")]
        public async Task<IActionResult> GetHouseMovers()
        {
            var response = await _houseMoverService.GetHouseMovers();
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost("add-package")]
        public async Task<IActionResult> AddPackage([FromForm]HouseMoverPackageRequest request, Guid? userId = null)
        {
            var response = await _houseMoverService.AddPackage(request, userId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("get-packages")]
        public async Task<IActionResult> GetPackages(Guid? userId = null)
        {
            var response = await _houseMoverService.GetPackages(userId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost("add-truck")]
        public async Task<IActionResult> AddTruck([FromForm]HouseMovingTruckRequest request,  Guid? userId = null)
        {
            var response = await _houseMoverService.AddTruck(request, userId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("get-trucks")]
        public async Task<IActionResult> GetTrucks(Guid? userId = null)
        {
            var response = await _houseMoverService.GetTrucks(userId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPatch("set-truck-price/{truckId}")]
        public async Task<IActionResult> SettruckPrice(Guid truckId, decimal price)
        {
            var response = await _houseMoverService.SetTruckPricing(truckId, price);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
