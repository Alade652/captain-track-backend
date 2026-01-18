using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.Courier;
// using CaptainTrackBackend.Application.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.Courier
{
    public interface ICourierService
    {
        Task<Response<CourierServiceDto>> InitBooking (Guid customerId, CourierServiceRequest request, Guid? riderorparkUserId);
        Task<Response<CourierServiceDto>> Book(Guid courierServiceId);
        Task<Response<IEnumerable<CourierServiceDto>>> GetBookings(Guid riderorParkUserIdORcustomerId);
        Task<Response<IEnumerable<CourierServiceDto>>> GetPendings(string location);
        Task<Response<bool>> CancelBooking(Guid courierServiceId);
        Task<Response<bool>> AcceptBooking(Guid courierServiceId);
        Task<Response<CourierServiceDto>> AcceptOffer(Guid bookingId, Guid riderUserId, decimal offerAmount);
        Task<Response<CourierServiceDto>> RaisePrice(Guid bookingId, decimal newPrice);

    }
}
