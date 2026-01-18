using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.VehicleTowing;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.VehicleTowing
{
    public interface ITruckOperatorService
    {
        Task<Response<ServiceProviderDto>> RegisterStoreOwner(Guid userId, ServiceProviderRequest request, IFormFileCollection files);
        Task<Response<FreelancerDto>> RegisterFreelancer(Guid userId, FreelancerRequest request, IFormFile file);
        Task<Response<TruckOperatorDto>> Get(Guid Id);
        Task<Response<TruckOperatorDto>> GetByUserId(Guid userId);
        Task<Response<List<TruckOperatorDto>>> GetAll();
        Task<Response<TruckDto>> AddTruck(TruckRequest request, Guid? truckOperatorUserId = null);
        Task<Response<List<TruckDto>>> GetTrucks(Guid? truckOperatorUserId = null);
    }
}
