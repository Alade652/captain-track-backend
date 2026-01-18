using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.WaterSupply
{
    public interface IWaterSupplierService
    {
        Task<Response<ServiceProviderDto>> RegisterStoreOwner(Guid userId, ServiceProviderRequest request, IFormFileCollection files);
        Task<Response<FreelancerDto>> RegisterFreelancer(Guid userId, FreelancerRequest request, IFormFile file);
        Task<Response<decimal>> SetPrice(decimal price, Guid? userId = null);
        Task<Response<ServiceProviderDto>> GetWaterSupplier(Guid userId);
        Task<Response<IEnumerable<ServiceProviderDto>>> GetWaterSuppliers();
    }
}
