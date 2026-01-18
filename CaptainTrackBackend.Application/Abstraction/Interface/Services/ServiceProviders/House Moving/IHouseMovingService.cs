using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.House_Moving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.House_Moving
{
    public interface IHouseMovingService
    {
        Task<Response<HouseMovingDto>> InitiateBooking(Guid customerId, HouseMovingRequest request, Guid? houseMoverUserId = null);
        Task<Response<HouseMovingDto>> Book(Guid houseMovingId);
        Task<Response<IEnumerable<HouseMovingDto>>> GetPending(string location);
        Task<Response<IEnumerable<HouseMovingDto>>> GetBookings(Guid userId);
        Task<Response<bool>> AcceptBooking(Guid houseMovingId);
        Task<Response<bool>> RejectBooking(Guid houseMovingId);
        Task<Response<bool>> CancelBooking(Guid houseMovingId);
        Task<Response<HouseMovingDto>> AcceptOffer(Guid bookingId, Guid houseMoverUserId, decimal offerAmount);
        Task<Response<HouseMovingDto>> RaisePrice(Guid bookingId, decimal newPrice);

    }
}
