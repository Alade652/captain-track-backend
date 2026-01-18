using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.DryCleaning;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.DryCleaning
{
    public interface IDryCleanerService
    {
        Task<Response<ServiceProviderDto>> RegisterStoreOwner(Guid userId, ServiceProviderRequest request, IFormFileCollection files);
        Task<Response<FreelancerDto>> RegisterFreelancer(Guid userId, FreelancerRequest request, IFormFile file);
        Task<Response<ServiceProviderDto>> Get(Guid Id);
        Task<Response<ServiceProviderDto>> GetByUserId(Guid userId);
        Task<Response<List<ServiceProviderDto>>> GetAll();
    }
}
