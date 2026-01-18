using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.Ambulance;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.Ambulance
{
    public interface IAmbulanceSerivice
    {
        Task<Response<ServiceProviderDto>> RegisterStoreOwner(Guid userId, ServiceProviderRequest request, IFormFileCollection files);
        Task<Response<FreelancerDto>> RegisterFreelancer(Guid userId, FreelancerRequest request, IFormFile file);
        Task<Response<ServiceProviderDto>> GetAmbulanceByUserIdAsync(Guid UserId);
        Task<Response<IEnumerable<ServiceProviderDto>>> GetAllAmbulancesAsync();
        Task<Response<bool>> SetPrice(decimal price, Guid? userId = null);
    }
}
