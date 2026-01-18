using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.Courier;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.Courier
{
    public interface IRiderorParkService
    {
        Task<Response<ServiceProviderDto>> RegisterPark(Guid userId, ServiceProviderRequest request, IFormFileCollection files);
        Task<Response<FreelancerDto>> RegisterRider(Guid userId, FreelancerRequest request, IFormFile file);
        Task<Response<IEnumerable<ParkDto>>> GetParks();
        Task<Response<ParkDto>> GetPark(Guid userId);
        Task<Response<FreelancerDto>> GetRider(Guid userId);
        Task<Response<CourierVehicleDto>> AddItem(CourierVehicleRequest request, Guid? userId = null);
        Task<Response<IEnumerable<CourierVehicleDto>>> GetItems(Guid? userId = null);

    }
}
