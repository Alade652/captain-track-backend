using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Application.Services.FileUpload;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Domain.Enum;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Services.ServiceProviders
{
    internal class ServiceProviderService : IServiceProviderService
    {
        private readonly IDryCleanerRepository _dryCleanerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFileUploadService _uploadService;
        public ServiceProviderService(IDryCleanerRepository dryCleanerRepository, IUserRepository userRepository, IFileUploadService fileUploadService)
        {
            _dryCleanerRepository = dryCleanerRepository;
            _userRepository = userRepository;
            _uploadService = fileUploadService;
        }

        /*public async Task<Response<ServiceProviderDto>> RegisterStoreOwner(Guid userId, ServiceProviderDto request, IFormFileCollection files)
        {
            var user = await _userRepository.GetAsync(x => x.Id == userId);
            if (user == null)
            {
                return new Response<ServiceProviderDto>
                {
                    Message = "not found",
                    Success = false,
                };
            }

            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                var url = await _uploadService.UploadAsync(file);

                switch (file.Name)
                {
                    case "RCorNIN":
                        request.RCorNIN = url;
                        break;
                    case "BusinessLogoorImageUrl":
                        request.BusinessLogoorImageUrl = url;
                        break;
                }
            }

            var serviceProvider = new DryCleaner
            {
                UserId = userId,
                CompanyName = request.CompanyName,
                CompanyDescription = request.CompanyDescription,
                RCorNIN = request.RCorNIN,
                BusinessLogoorImageUrl = request.BusinessLogoorImageUrl,
                CompanyContact = request.CompanyContact,
                AddressorLocation = request.AddressorLocation,
                Owner = request.Owner,
                Locations = request.Locations,
                City = request.City,
                YearsOfService = request.YearsOfService,
                AccountName = request.AccountName,
                AccountNumber = request.AccountNumber,
                BankName = request.BankName,
                ServiceProviding = ServiceProviding.DryCleaning
            };

        }*/
    }
}
