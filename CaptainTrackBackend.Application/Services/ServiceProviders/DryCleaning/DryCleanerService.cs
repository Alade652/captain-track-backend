using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.Services.FileUpload;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.House_Moving;
using CaptainTrackBackend.Domain.Enum;
using CaptainTrackBackend.Domain.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CaptainTrackBackend.Application.Services.ServiceProviders.DryCleaning
{
    public class DryCleanerService : IDryCleanerService
    {
        private readonly IUnitofWork _unitofWork;
        private readonly IFileUploadService _fileUploadService;
        private readonly IEmailService _emailService;

        public DryCleanerService(IUnitofWork unitofWork, IFileUploadService fileUploadService, IEmailService emailService) 
        {
            _unitofWork = unitofWork;
            _fileUploadService = fileUploadService;
            _emailService = emailService;
        }

        public async Task<Response<ServiceProviderDto>> Get(Guid id)
        {
            var dryCleaner = await _unitofWork.DryCleaner.GetAsync(id);
            if (dryCleaner == null)
            {
                return new Response<ServiceProviderDto> { Message = "Does not exist", Success = false };
            }

            var dryCleanerDto = new ServiceProviderDto
            {
                CompanyName = dryCleaner.CompanyName,
                CompanyContact = dryCleaner.BusinessContact,
                Address = dryCleaner.AddressorLocation,
                City = dryCleaner.City,
                Locations = dryCleaner.Locations,
                BusinessLogoUrl = dryCleaner.BusinessLogoorImageUrl,
                RC = dryCleaner.RCorNIN,
                AccountName = dryCleaner.AccountName,
                AccountNumber = dryCleaner.AccountNumber,
                BankName = dryCleaner.BankName,
                YearsOfService = dryCleaner.YearsOfService,
                Owner = dryCleaner.Owner,
                ServiceProviderRole = dryCleaner.ServiceProviderRole.ToString()
            };
            return new Response<ServiceProviderDto>
            {
                Message = "Driver gotten",
                Success = true,
                Data = dryCleanerDto
            };
        }

        public async Task<Response<ServiceProviderDto>> GetByUserId(Guid userId)
        {
            var dryCleaner = await _unitofWork.DryCleaner.GetAsync(x => x.UserId == userId);
            if (dryCleaner == null)
            {
                return new Response<ServiceProviderDto> { Message = "Does not exist", Success = false };
            }

            var dryCleanerDto = new ServiceProviderDto
            {
                CompanyName = dryCleaner.CompanyName,
                CompanyContact = dryCleaner.BusinessContact,
                Address = dryCleaner.AddressorLocation,
                City = dryCleaner.City,
                Locations = dryCleaner.Locations,
                BusinessLogoUrl = dryCleaner.BusinessLogoorImageUrl,
                RC = dryCleaner.RCorNIN,
                AccountName = dryCleaner.AccountName,
                AccountNumber = dryCleaner.AccountNumber,
                BankName = dryCleaner.BankName,
                YearsOfService = dryCleaner.YearsOfService,
                Owner = dryCleaner.Owner,
            };
            return new Response<ServiceProviderDto>
            {
                Message = "Driver gotten",
                Success = true,
                Data = dryCleanerDto
            };
        }

        public async Task<Response<ServiceProviderDto>> RegisterStoreOwner(Guid userId, ServiceProviderRequest request, IFormFileCollection files)
        {
            var user = await _unitofWork.User.GetAsync(x => x.Id == userId);
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

                var url = await _fileUploadService.UploadAsync(file);

                switch (file.Name)
                {
                    case "BusinessLogoUrl":
                        request.BusinessLogoUrl = url;
                        break;
                }
            }

            // Check if dry cleaner already exists for this user
            var existingDryCleaner = await _unitofWork.DryCleaner.GetAsync(x => x.UserId == userId);
            DryCleaner dryCleaner;
            
            if (existingDryCleaner != null)
            {
                // Update existing record
                dryCleaner = existingDryCleaner;
                dryCleaner.CompanyName = request.CompanyName;
                dryCleaner.RCorNIN = request.RC;
                dryCleaner.BusinessLogoorImageUrl = request.BusinessLogoUrl;
                dryCleaner.BusinessContact = request.CompanyContact;
                dryCleaner.AddressorLocation = request.Address;
                dryCleaner.Owner = request.Owner;
                dryCleaner.Locations = request.Locations;
                dryCleaner.City = request.City;
                dryCleaner.YearsOfService = request.YearsOfService;
                dryCleaner.AccountName = request.AccountName;
                dryCleaner.AccountNumber = request.AccountNumber;
                dryCleaner.BankName = request.BankName;
                dryCleaner.ServiceProviderRole = ServiceProviderRole.StoreOwner;
                await _unitofWork.DryCleaner.UpdateAsync(dryCleaner);
            }
            else
            {
                // Create new record
                dryCleaner = new DryCleaner
                {
                    UserId = userId,
                    CompanyName = request.CompanyName,
                    RCorNIN = request.RC,
                    BusinessLogoorImageUrl = request.BusinessLogoUrl,
                    BusinessContact = request.CompanyContact,
                    AddressorLocation = request.Address,
                    Owner = request.Owner,
                    Locations = request.Locations,
                    City = request.City,
                    YearsOfService = request.YearsOfService,
                    AccountName = request.AccountName,
                    AccountNumber = request.AccountNumber,
                    BankName = request.BankName,
                    ServiceProviderRole = ServiceProviderRole.StoreOwner
                };
                await _unitofWork.DryCleaner.AddAsync(dryCleaner);
            }

            var service = await _unitofWork.Context.ServiceProvidings
                .FirstOrDefaultAsync(x => x.Name == "Dry Cleaning");
            if (service == null)
            {
                service = new ServiceProviding
                {
                    Name = "Dry Cleaning",
                    Description = "Dry Cleaning Service",
                    //ImageUrl = "default_image_url" // Replace with actual default image URL
                };
                await _unitofWork.Context.ServiceProvidings.AddAsync(service);
                await _unitofWork.Context.SaveChangesAsync();
            }
            //user.ServiceProvidings.Add(service);
            user.ServiceProviderRole = dryCleaner.ServiceProviderRole;
            await _unitofWork.User.UpdateAsync(user);

            // Check if UserServiceProviding already exists
            var existingUserServiceProviding = await _unitofWork.Context.UserServiceProvidings
                .FirstOrDefaultAsync(x => x.UserId == user.Id && x.ServiceProvidingId == service.Id);
            
            if (existingUserServiceProviding == null)
            {
                var userServiceproviding = new UserServiceProviding
                {
                    UserId = user.Id,
                    ServiceProvidingId = service.Id,
                };
                await _unitofWork.Context.UserServiceProvidings.AddAsync(userServiceproviding);
            }
            
            await _unitofWork.Context.SaveChangesAsync();
            var emailDto = new EmailDto
            {
                To = user.Email,
                Subject = "Welcome to CaptainTrack",
                Body = $"Hello {user.FullName},<br/>You've registered on CaptainTrack. Pls await verification"
            };
            await _emailService.SendEmailAsync(emailDto);

            return new Response<ServiceProviderDto>
            {
                Message = "Registration Succesful",
                Success = true,
                Data = new ServiceProviderDto
                {
                    UserId = user.Id,
                    Id = dryCleaner.Id,
                    CompanyName = dryCleaner.CompanyName,
                    City = dryCleaner.City,
                    RC = dryCleaner.RCorNIN,
                    CompanyContact = dryCleaner.BusinessContact,
                    Address = dryCleaner.AddressorLocation,
                    Owner = dryCleaner.Owner,
                    Locations = dryCleaner.Locations,
                    YearsOfService = dryCleaner.YearsOfService,
                    BusinessLogoUrl = dryCleaner.BusinessLogoorImageUrl,
                    AccountName = request.AccountName,
                    AccountNumber = request.AccountNumber,
                    BankName = request.BankName,
                    ServiceProviderRole = dryCleaner.ServiceProviderRole.ToString(),

                }
            };
        }

        public async Task<Response<List<ServiceProviderDto>>> GetAll()
        {
            var dryCleaners = await _unitofWork.DryCleaner.GetAllAsync();
            if (dryCleaners == null)
            {
                return new Response<List<ServiceProviderDto>>
                {
                    Message = "No dry cleaners found",
                    Success = false,
                    Data = new List<ServiceProviderDto>()
                };
            }
            var dryCleanerDtos = dryCleaners.Select(d => new ServiceProviderDto
            {
                Id = d.Id,
                UserId = d.UserId,
                CompanyName = d.CompanyName,
                CompanyContact = d.BusinessContact,
                Address = d.AddressorLocation,
                City = d.City,
                Locations = d.Locations,
                BusinessLogoUrl = d.BusinessLogoorImageUrl,
                RC = d.RCorNIN,
                AccountName = d.AccountName,
                AccountNumber = d.AccountNumber,
                BankName = d.BankName,
                YearsOfService = d.YearsOfService,
                Owner = d.Owner,
                ServiceProviderRole = d.ServiceProviderRole.ToString()
            }).ToList();
            return new Response<List<ServiceProviderDto>>
            {
                Message = "Dry cleaners retrieved successfully",
                Success = true,
                Data = dryCleanerDtos
            };
            //var dryCleaners = await _dryCleanerRepository.GetAllAsync(x => x.City == location);
        }

        public async Task<Response<FreelancerDto>> RegisterFreelancer(Guid userId, FreelancerRequest request, IFormFile file)
        {
            var user = await _unitofWork.User.GetAsync(x => x.Id == userId);
            if (user == null)
            {
                return new Response<FreelancerDto>
                {
                    Message = "not found",
                    Success = false,
                };
            }
            if (file != null)
            {
                request.ImageUrl = await _unitofWork.FileUpload.UploadAsync(file);
            }
            
            // Check if dry cleaner already exists for this user
            var existingDryCleaner = await _unitofWork.DryCleaner.GetAsync(x => x.UserId == userId);
            DryCleaner dryCleaner;
            
            if (existingDryCleaner != null)
            {
                // Update existing record
                dryCleaner = existingDryCleaner;
                dryCleaner.BusinessContact = request.BusinessContact;
                dryCleaner.RCorNIN = request.NIN;
                dryCleaner.AddressorLocation = request.Location;
                dryCleaner.City = request.City;
                dryCleaner.BankName = request.BankName;
                dryCleaner.AccountName = request.AccountName;
                dryCleaner.AccountNumber = request.AccountNumber;
                dryCleaner.BusinessLogoorImageUrl = request.ImageUrl;
                dryCleaner.ServiceProviderRole = ServiceProviderRole.Freelancer;
                await _unitofWork.DryCleaner.UpdateAsync(dryCleaner);
            }
            else
            {
                // Create new record
                dryCleaner = new DryCleaner
                {
                    UserId = userId,
                    BusinessContact = request.BusinessContact,
                    RCorNIN = request.NIN,
                    AddressorLocation = request.Location,
                    City = request.City,
                    BankName = request.BankName,
                    AccountName = request.AccountName,
                    AccountNumber = request.AccountNumber,
                    BusinessLogoorImageUrl = request.ImageUrl,
                    ServiceProviderRole = ServiceProviderRole.Freelancer
                };
                await _unitofWork.DryCleaner.AddAsync(dryCleaner);
            }

            var service = await _unitofWork.Context.ServiceProvidings
                .FirstOrDefaultAsync(x => x.Name == "Dry Cleaning");
            if (service == null)
            {
                service = new ServiceProviding
                {
                    Name = "Dry Cleaning",
                    Description = "Dry MovCleaninging Service",
                    //ImageUrl = "default_image_url" // Replace with actual default image URL
                };
                await _unitofWork.Context.ServiceProvidings.AddAsync(service);
                await _unitofWork.Context.SaveChangesAsync();
            }
            //user.ServiceProvidings.Add(service);
            user.ServiceProviderRole = dryCleaner.ServiceProviderRole;
            await _unitofWork.User.UpdateAsync(user);

            // Check if UserServiceProviding already exists
            var existingUserServiceProviding = await _unitofWork.Context.UserServiceProvidings
                .FirstOrDefaultAsync(x => x.UserId == user.Id && x.ServiceProvidingId == service.Id);
            
            if (existingUserServiceProviding == null)
            {
                var userServiceproviding = new UserServiceProviding
                {
                    UserId = user.Id,
                    ServiceProvidingId = service.Id,
                };
                await _unitofWork.Context.UserServiceProvidings.AddAsync(userServiceproviding);
            }
            
            await _unitofWork.Context.SaveChangesAsync();
            var emailDto = new EmailDto
            {
                To = user.Email,
                Subject = "Welcome to CaptainTrack",
                Body = $"Hello {user.FullName},<br/>You've registered on CaptainTrack. Pls await verification"
            };
            await _emailService.SendEmailAsync(emailDto);

            return new Response<FreelancerDto>
            {
                Message = "Registered Successfully",
                Success = true,
                Data = new FreelancerDto
                {
                    Id = dryCleaner.Id,
                    UserId = user.Id,
                    Name = user.FullName,
                    Email = user.Email,
                    BusinessContact = dryCleaner.BusinessContact,
                    NIN = dryCleaner.RCorNIN,
                    Location = dryCleaner.AddressorLocation,
                    City = dryCleaner.City,
                    BankName = dryCleaner.BankName,
                    AccountName = dryCleaner.AccountName,
                    AccountNumber = dryCleaner.AccountNumber,
                    ImageUrl = dryCleaner.BusinessLogoorImageUrl,
                    ServiceProviderRole = dryCleaner.ServiceProviderRole.ToString()
                }
            };
        }
    }
}
