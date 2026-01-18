using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.GasDelivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.GasDelivery
{
    public interface IGasDeliveryService
    {
        Task<Response<GasDeliveryDto>> InitBooking(Guid customerId, Guid gasSupplierUserId, GasDeliveryRequestDto request);
        Task<Response<GasDeliveryDto>> InitBooking(Guid customerId, GasDeliveryRequestDto request);
        Task<Response<GasDeliveryDto>> Book(Guid gasDeliveryId);
        Task<Response<List<GasDeliveryDto>>> GetPending(string location);
        Task<Response<List<GasDeliveryDto>>> GetBookings(Guid supplierUserId);
        Task<Response<bool>> AcceptBooking(Guid gasDeliveryId);
        Task<Response<bool>> RejectBooking(Guid gasDeliveryId);
        Task<Response<bool>> CancelBooking(Guid gasDeliveryId);
        Task<Response<GasDeliveryDto>> AcceptOffer(Guid bookingId, Guid gasSupplierUserId, decimal offerAmount);
        Task<Response<GasDeliveryDto>> RaisePrice(Guid bookingId, decimal newPrice);



    }
}
