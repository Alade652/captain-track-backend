using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders.WaterSupply;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.WaterSupply
{
    public interface IWaterSupplingService
    {
        Task<Response<WaterSupplingDto>> InitBooking(WaterSupplingRequest request, Guid customerId, Guid? waterSupplierUserId = null);
        Task<Response<WaterSupplingDto>> Book(Guid waterSupplingId);
        Task<Response<IEnumerable<WaterSupplingDto>>> GetBookings(Guid userId);
        Task<Response<IEnumerable<WaterSupplingDto>>> GetPending(string location);
        Task<Response<WaterSupplingDto>> GetBookingById(Guid waterSupplingId);
        Task<Response<bool>> CancelBooking(Guid waterSupplingId);
        Task<Response<bool>> AcceptBooking(Guid waterSupplingId);
        Task<Response<bool>> RejectBooking(Guid waterSupplingId);
        Task<Response<WaterSupplingDto>> AcceptOffer(Guid bookingId, Guid waterSupplierUserId, decimal offerAmount);
        Task<Response<WaterSupplingDto>> RaisePrice(Guid bookingId, decimal newPrice);
    }
}
