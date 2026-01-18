using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.DryCleaning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.DryCleaning
{
    public interface IDryCleaningService
    {
        Task<Response<DryCleaningDto>> InitBook(Guid customerId, Guid dryCleanerUserid, DryCleaningRequestDto dryCleaningrequest);
        Task<Response<DryCleaningDto>> InitBookFreelancer(Guid customerId, DryCleaningRequestDto dryCleaningrequest);
        Task<Response<DryCleaningDto>> Book(Guid dryCleaningId);
        Task<Response<List<DryCleaningDto>>> GetPending(string location);
        Task<Response<List<DryCleaningDto>>> GetBookings(Guid userId);
        Task<Response<bool>> AccepBooking(Guid dryCleaningId);
        Task<Response<bool>> RejectBooking(Guid dryCleaningId);
        Task<Response<bool>> CancelBooking(Guid dryCleaningId);
        Task<Response<DryCleaningDto>> AcceptOffer(Guid bookingId, Guid dryCleanerUserId, decimal offerAmount);
        Task<Response<DryCleaningDto>> RaisePrice(Guid bookingId, decimal newPrice);
    }
}
