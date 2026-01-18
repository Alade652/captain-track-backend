using CaptainTrackBackend.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services
{
    public interface IRatingService
    {
        Task<Response<RatingDto>> CreateRating(Guid createdBy, RatingRequest request);
        Task<Response<IList<RatingDto>>> GetCustomerRatings(Guid customerId);
        Task<Response<IList<RatingDto>>> GetVendorRatings(Guid vendorId);
        //Task<Response<IList<RatingDto>>> GetRatings(Guid customerId, Guid vendorId);
        Task<Response<bool>> DeleteRating(Guid userId);
    }
}
