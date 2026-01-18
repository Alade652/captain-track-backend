using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.CarWash;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.CarWash;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers.ServiceProvider.CarWash
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarWashingcontroller : ControllerBase
    {
        private readonly ICarWashingService _carWashingService;
        public CarWashingcontroller(ICarWashingService carWashingService)
        {
            _carWashingService = carWashingService;
        }

        [HttpPost("initiate-booking/{customerId}")]
        public async Task<IActionResult> InitiateBooking(Guid customerId, CarWashingRequest request, Guid? carWasherUserId = null)
        {
            var response = await _carWashingService.InitiateBooking(customerId, request, carWasherUserId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPatch("book/{carWashingId}")]
        public async Task<IActionResult> Book(Guid carWashingId)
        {
            var response = await _carWashingService.Book(carWashingId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPatch("accept-booking/{carWashingId}")]
        public async Task<IActionResult> AcceptBooking(Guid carWashingId)
        {
            var response = await _carWashingService.AcceptBooking(carWashingId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPatch("cancel-booking/{carWashingId}")]
        public async Task<IActionResult> CancelBooking(Guid carWashingId)
        {
            var response = await _carWashingService.CancelBooking(carWashingId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPatch("reject-booking/{carWashingId}")]
        public async Task<IActionResult> RejectBooking(Guid carWashingId)
        {
            var response = await _carWashingService.RejectBooking(carWashingId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("get-pendings/{location}")]
        public async Task<IActionResult> GetPendings(string location)
        {
            var response = await _carWashingService.GetPendings(location);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("get-carWashings/{userId}")]
        public async Task<IActionResult> GetCarWashings(Guid userId)
        {
            var response = await _carWashingService.GetBookings(userId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPatch("accept-offer/{carWashingId}")]
        public async Task<IActionResult> AcceptOffer(Guid carWashingId, Guid carWasherUserId, decimal offerAmount)
        {
            var response = await _carWashingService.AcceptOffer(carWashingId, carWasherUserId, offerAmount);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPatch("raise-fare/{carWashingId}")]
        public async Task<IActionResult> RaiseFare(Guid carWashingId, decimal amount)
        {
            var response = await _carWashingService.RaisePrice(carWashingId, amount);
            return response.Success ? Ok(response) : BadRequest(response);
        }
            

    }
}
