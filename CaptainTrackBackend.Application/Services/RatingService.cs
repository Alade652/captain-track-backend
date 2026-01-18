using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders;
using CaptainTrackBackend.Application.Abstraction.Interface.Services;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Domain.Entities;
using CaptainTrackBackend.Domain.Enum;

namespace CaptainTrackBackend.Application.Services
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        public RatingService(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }
        public async Task<Response<RatingDto>> CreateRating(Guid createdBy, RatingRequest request)
        {

            var rating = new Rating
            {
                CustomerId = request.CustomerId,
                UserId = request.VendorUserId,
                Stars = request.Stars,
                Comment = request.Comment,
                CreatedBy = createdBy,
                CreatedOn = DateTime.UtcNow,
                ServiceType = request.ServiceType
            };
            await _ratingRepository.AddAsync(rating);

            return new Response<RatingDto>
            {
                Message = "Rating created successfully",
                Success = true,
                Data = new RatingDto
                {
                    Id = rating.Id,
                    Stars = rating.Stars,
                    Comment = rating.Comment,
                    CustomerId = rating.CustomerId,
                    VendorUserId = rating.UserId,
                    ServiceType= rating.ServiceType,
                    CreatedBy = rating.CreatedBy,

                }
            };
        }

        public Task<Response<bool>> DeleteRating(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<IList<RatingDto>>> GetCustomerRatings(Guid customerId)
        {
            var ratings = await _ratingRepository.GetAllAsync(x => x.CustomerId == customerId);

            if (ratings == null || ratings.Count == 0)
            {
                return new Response<IList<RatingDto>>
                {
                    Message = "No ratings found",
                    Success = false,
                    Data = null
                };
            }
            var ratingDtos = ratings.Select(r => new RatingDto
            {
                Id = r.Id,
                Stars = r.Stars,
                Comment = r.Comment,
                CustomerId = r.CustomerId,
                VendorUserId = r.UserId,
                ServiceType= r.ServiceType,
                CreatedBy = r.CreatedBy,    
            }).ToList();

            return new Response<IList<RatingDto>>
            {
                Message = "Ratings found successfully",
                Success = true,
                Data = ratingDtos
            };
        }

        /*public async Task<Response<IList<RatingDto>>> GetRatings(Guid customerId, Guid vendorId)
        {
            var ratings = await _ratingRepository.GetAllAsync(customerId, vendorId);

            if (ratings == null || ratings.Count == 0)
            {
                return new Response<IList<RatingDto>>
                {
                    Message = "No ratings found",
                    Success = false,
                    Data = null
                };
            }
            var ratingDtos = ratings.Select(r => new RatingDto
            {
                Id = r.Id,
                Stars = (int)r.Stars,
                Comment = r.Comment,
                CustomerId = r.CustomerId,
                VendorUserId = r.UserId,
                ServiceType= r.ServiceType,
                CreatedBy = r.CreatedBy,
            }).ToList();

            return new Response<IList<RatingDto>>
            {
                Message = "Ratings found successfully",
                Success = true,
                Data = ratingDtos
            };
        }
*/
        public async Task<Response<IList<RatingDto>>> GetVendorRatings(Guid vendorId)
        {
            var ratings = await _ratingRepository.GetAllAsync(x => x.UserId == vendorId);

            if (ratings == null || ratings.Count == 0)
            {
                return new Response<IList<RatingDto>>
                {
                    Message = "No ratings found",
                    Success = false,
                    Data = null
                };
            }
            var ratingDtos = ratings.Select(r => new RatingDto
            {
                Id = r.Id,
                Stars = r.Stars,
                Comment = r.Comment,
                CustomerId = r.CustomerId,
                VendorUserId = r.UserId,
                ServiceType= r.ServiceType,
                CreatedBy = r.CreatedBy,
            }).ToList();

            return new Response<IList<RatingDto>>
            {
                Message = "Ratings found successfully",
                Success = true,
                Data = ratingDtos
            };
        }
    }
}
