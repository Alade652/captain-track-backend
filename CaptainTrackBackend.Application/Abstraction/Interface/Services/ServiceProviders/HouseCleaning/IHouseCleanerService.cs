using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.HouseCleaning;
using Microsoft.AspNetCore.Http;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.HouseCleaning
{
    public interface IHouseCleanerService
    {
        Task<Response<ServiceProviderDto>> RegisterStoreOwner(Guid userId, ServiceProviderRequest request, IFormFileCollection file);
        Task<Response<FreelancerDto>> RegisterFreelancer(Guid userId, FreelancerRequest request, IFormFile file);
        Task<Response<List<ServiceProviderDto>>> GetHouseCleaners();
        Task<Response<ServiceProviderDto>> GetHouseCleaner(Guid userId);
        Task<Response<HouseCleanerPackageDto>> AddPackage(HouseCleanerPackageRequest request, Guid? houseCleanerUserId = null);
        Task<Response<List<HouseCleanerPackageDto>>> GetPackages(Guid? houseCleanerUserId);
        Task<Response<HouseCleanerItemDto>> AddItem(Guid houseCleanerPackageId, HouseCleanerItemRequest request);
        Task<Response<List<HouseCleanerItemDto>>> GetItems(Guid houseCleanerPackageId);
        Task<Response<HouseCleanerItemDto>> UpdateItemPrice(Guid houseCleanerItemId, decimal price);
    }
}
