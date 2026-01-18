using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.GasDelivery;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.GasDelivery
{
    public interface IGasSupplierService
    {
        Task<Response<string>> SetPricePerKg(decimal pricePerKg);
        Task<Response<ServiceProviderDto>> Register(Guid userId, ServiceProviderRequest request, IFormFileCollection files);
        Task<Response<FreelancerDto>> RegisterFreelancer(Guid userId, FreelancerRequest request, IFormFile file);
        Task<Response<ServiceProviderDto>> GetGasSupplierByUserId(Guid userId);
        Task<Response<List<ServiceProviderDto>>> GetAllGasSuppliers();
        Task<Response<string>> SetPrice(decimal pricePerKg, Guid gasSupplierUserId);
        //Task<Response<bool>> Update(Guid gasSupplierUserId);
        //Task<Response<GasSupplierDto>> Update(Guid gasSupplierUserId, GasSupplierUpdateRequest request, IFormFileCollection files);

    }
}
