using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.House_Moving;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.House_Moving
{
    public interface IHouseMoverService
    {
        Task<Response<ServiceProviderDto>> RegisterStoreOwner(Guid userId, ServiceProviderRequest request, IFormFileCollection files);
        Task<Response<FreelancerDto>> RegisterFreeLancer(Guid userId, FreelancerRequest request, IFormFile file);
        Task<Response<IEnumerable<ServiceProviderDto>>> GetHouseMovers();
        Task<Response<ServiceProviderDto>> GetHouseMover(Guid userId);
        Task<Response<HouseMoverPackageDto>> AddPackage(HouseMoverPackageRequest request, Guid? houseMoverUserId = null);
        Task<Response<IEnumerable<HouseMoverPackageDto>>> GetPackages(Guid? userId = null);
        Task<Response<HouseMovingTruckDto>> AddTruck(HouseMovingTruckRequest request, Guid? houseMoverUserId = null);
        Task<Response<IEnumerable<HouseMovingTruckDto>>> GetTrucks(Guid? userId = null);
        Task<Response<decimal>> SetTruckPricing(Guid truckId, decimal price);
    }
}
