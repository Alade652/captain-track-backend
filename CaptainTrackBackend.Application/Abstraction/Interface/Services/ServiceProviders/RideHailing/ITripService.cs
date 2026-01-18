using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.Ridehailing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.RideHailing
{
    public interface ITripService
    {
        Task<Response<TripDto>> InitiateTrip(TripRequestDto tripRequestDto, Guid customerId);
        Task<Response<TripDto>> Book(Guid tripId);

        Task<Response<TripDto>> GetTripById(Guid tripId);
        Task<Response<bool>> StartTrip(Guid tripId);
        Task<Response<string>> EndTrip(Guid tripId);
        Task<Response<bool>> CancelTrip(Guid tripId, Guid userId);
        Task<Response<string>> AddStop(Guid tripId, string location);
        Task<Response<string>> GetDistanceBetweenDriverAndCustomer(Guid tripId, string driverLocation);
        Task<Response<IList<TripDto>>> GetPendingTrips(string location);
        Task<Response<IList<TripDto>>> GetTrips(Guid userId);
        Task<Response<bool>> RejectBooking(Guid tripId);
        Task<Response<TripDto>> AcceptOffer(Guid tripId, Guid driverUserId, decimal offerAmount);
        Task<Response<TripDto>> RaiseFare(Guid tripId, decimal newFare);


    }
}
