using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.HouseCleaning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.HouseCleaning
{
    public interface IHouseCleaningService
    {
        Task<Response<HouseCleaningDto>> InitBooking(Guid customerId,  HouseCleaningRequest request, Guid? houseCleanerUserId = null);
        Task<Response<HouseCleaningDto>> Book(Guid houseCleaningid);
        Task<Response<List<HouseCleaningDto>>> GetHouseCleanings(Guid houseCleanerUserid);
        Task<Response<List<HouseCleaningDto>>> GetPending(string location);
        Task<Response<bool>> AcceptBooking(Guid houseCleaningId);
        Task<Response<bool>> CancelBooking(Guid houseCleaningId);
        Task<Response<bool>> RejectBooking(Guid houseCleaningId);
        Task<Response<HouseCleaningDto>> AcceptOffer(Guid bookingId, Guid houseCleanerUserId, decimal offerAmount);
        Task<Response<HouseCleaningDto>> RaisePrice(Guid bookingId, decimal newPrice);
    }
}
