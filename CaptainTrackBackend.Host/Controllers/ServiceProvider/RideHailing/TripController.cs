using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.RideHailing;
using CaptainTrackBackend.Application.DTO.ServiceProviders.Ridehailing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers.ServiceProvider.RideHailing
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        private readonly ITripService _tripService;
        public TripController(ITripService tripService)
        {
            _tripService = tripService;
        }



        /*        [HttpPost("add-stop/{tripId}")]
                public async Task<IActionResult> AddStop([FromRoute]Guid tripId, [FromBody] string location)
                {
                    var result = await _tripService.AddStop(tripId, location);
                    if (result.Success)
                    {
                        return Ok(result);
                    }
                    return BadRequest(result);
                }*/

        [HttpPost("InitiateTrip/{customerId}")]
        public async Task<IActionResult> InitiateTrip([FromRoute] Guid customerId, [FromBody] TripRequestDto tripDTO)
        {
            var result = await _tripService.InitiateTrip(tripDTO, customerId);
            return result.Success? Ok(result) : BadRequest(result);
        }

        [HttpPost("book/{tripId}")]
        public async Task<IActionResult> BookTrip([FromRoute] Guid tripId)
        {
            var result = await _tripService.Book(tripId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("start/{tripId}")]
        public async Task<IActionResult> StartTrip([FromRoute] Guid tripId)
        {
            var result = await _tripService.StartTrip(tripId);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpPost("end/{tripId}")]
        public async Task<IActionResult> EndTrip([FromRoute] Guid tripId)
        {
            var result = await _tripService.EndTrip(tripId);
            return result.Success ? Ok(result) : BadRequest(result);
        }



        [HttpPost("cancel/{tripId}/{userId}")]
        public async Task<IActionResult> CancelTrip([FromRoute] Guid tripId, [FromRoute] Guid userId)
        {
            var result = await _tripService.CancelTrip(tripId, userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /*        [HttpGet("getDistanceBetweenDriverAndCustomer/{tripId}")]
                public async Task<IActionResult> GetDistanceBetweenDriverAndCustomer([FromRoute] Guid tripId, string driverLocation)
                {
                    var result = await _tripService.GetDistanceBetweenDriverAndCustomer(tripId, driverLocation);
                    return result.Success ? Ok(result) : BadRequest(result);
                }*/

        [HttpGet("getPending/{location}")]
        public async Task<IActionResult> GetPendingTrips(string location)
        {
            var result = await _tripService.GetPendingTrips(location);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpGet("{tripId}")]
        public async Task<IActionResult> GetTripById([FromRoute] Guid tripId)
        {
            var result = await _tripService.GetTripById(tripId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-trips/{userId}")]
        public async Task<IActionResult> GetTrips([FromRoute] Guid userId)
        {
            var result = await _tripService.GetTrips(userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpPost("acceptOffer/{tripId}")]
        public async Task<IActionResult> AcceptOffer(Guid tripId, Guid driverUserId, decimal offerAmount)
        {
            var result = await _tripService.AcceptOffer(tripId, driverUserId, offerAmount);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("raise-fare/{tripId}")]
        public async Task<IActionResult> RaiseFare(Guid tripId, decimal amount)
        {
            var response = await _tripService.RaiseFare(tripId, amount);
            return response.Success ? Ok(response) : BadRequest(response);
        }

    }
}
