using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.Ambulance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.Ambulance
{
    public interface IAmbulanceBookingService
    {
        Task<Response<AmbulanceBookingDto>> InitiateAmbulanceBookingAsync(Guid userId, AmbulanceBookingRequest request, Guid? ambulanceUserId = null);
        Task<Response<AmbulanceBookingDto>> Book(Guid bookingId);
        Task<Response<IEnumerable<AmbulanceBookingDto>>> GetBookingsByUserIdAsync(Guid userId);
        Task<Response<AmbulanceBookingDto>> GetBookingDetailsAsync(Guid bookingId);
        Task<Response<IEnumerable<AmbulanceBookingDto>>> GetPendings(string location);
        Task<Response<bool>> AcceptBookingAsync(Guid bookingId);
        Task<Response<bool>> CancelBookingAsync(Guid bookingId);
        Task<Response<AmbulanceBookingDto>> AcceptOffer(Guid bookingId, Guid ambulanceUserId, decimal offerAmount);
        Task<Response<AmbulanceBookingDto>> RaisePrice(Guid bookingId, decimal newPrice);



    }
}
