using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.VehicleTowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.VehicleTowing
{
    public interface ITowingService 
    {
        Task<Response<TowingDto>> InitBooking(Guid customerId, TowingRequest request, Guid? truckOperatorUserId = null);
        Task<Response<TowingDto>> Book(Guid towingId);
        Task<Response<List<TowingDto>>> GetPendings(string location);
        Task<Response<List<TowingDto>>> GetBookings(Guid truckOperatorUserId);
        Task<Response<bool>> AcceptBooking(Guid towingId);
        Task<Response<bool>> RejectBooking(Guid towingId);
        Task<Response<bool>> CancelBooking(Guid towingId);
        Task<Response<TowingDto>> AcceptOffer(Guid bookingId, Guid truckOperatorUserId, decimal offerAmount);
        Task<Response<TowingDto>> RaisePrice(Guid bookingId, decimal newPrice);
    }
}
