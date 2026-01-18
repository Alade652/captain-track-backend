using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.CarWash;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.CarWash
{
    public interface ICarWasherService
    {
        public Task<Response<ServiceProviderDto>> RegisterStoreowner(Guid userId, ServiceProviderRequest request, IFormFileCollection files);
        public Task<Response<FreelancerDto>> RegisterFreelancer(Guid userId, FreelancerRequest request, IFormFile file);
        public Task<Response<CarWashDto>> GetCarWash(Guid userId);
        public Task<Response<IEnumerable<CarWashDto>>> GetCarwashes();
        public Task<Response<CarWashItemDto>> AddCarWashItem(CarWashItemRequest request, Guid? userId = null);
        public Task<Response<IEnumerable<CarWashItemDto>>> GetCarWashItems(Guid? userId = null);
        public Task<Response<CarWashItemDto>> UpdateCarWashItem(Guid itemId, CarWashItemRequest request);
    }
}
