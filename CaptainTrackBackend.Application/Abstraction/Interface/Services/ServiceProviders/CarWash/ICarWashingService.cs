using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.CarWash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.CarWash
{
    public interface ICarWashingService
    {
        Task<Response<CarWashingDto>> InitiateBooking (Guid customerId, CarWashingRequest request, Guid? carWasherUserId = null);
        Task<Response<CarWashingDto>> Book(Guid carWashingId);
        Task<Response<IEnumerable<CarWashingDto>>> GetPendings(string location);
        Task<Response<IEnumerable<CarWashingDto>>> GetBookings(Guid userId);
        Task<Response<bool>> AcceptBooking(Guid carWashingId);
        Task<Response<bool>> RejectBooking(Guid carWashingId);
        Task<Response<bool>> CancelBooking(Guid carWashingId);
        Task<Response<CarWashingDto>> AcceptOffer(Guid bookingId, Guid carWasherUserId, decimal offerAmount);
        Task<Response<CarWashingDto>> RaisePrice(Guid bookingId, decimal newPrice);
    }
}
