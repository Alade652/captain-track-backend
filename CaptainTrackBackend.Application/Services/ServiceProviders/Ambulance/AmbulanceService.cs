using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.Ambulance;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Domain.Entities.ServiceProviders;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.Ambulance;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.House_Moving;
using CaptainTrackBackend.Domain.Enum;
using CaptainTrackBackend.Domain.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Services.ServiceProviders.Ambulance
{
    public class AmbulanceService : IAmbulanceSerivice
    {
        private readonly IUnitofWork _unitofWork;
        public AmbulanceService(IUnitofWork unitofWork)
        {
            _unitofWork = unitofWork;
        }
        public async Task<Response<IEnumerable<ServiceProviderDto>>> GetAllAmbulancesAsync()
        {
            var respomse = new Response<IEnumerable<ServiceProviderDto>>();
            var ambulances = await _unitofWork.Ambulance.GetAllAsync();
            if (ambulances == null || !ambulances.Any())
            {
                respomse.Success = false;
                respomse.Message = "No ambulances found";
                return respomse;
            }
            var ambulanceDtos = ambulances.Select(a => new ServiceProviderDto
            {
                Id = a.Id,
                UserId = a.UserId,
                CompanyName = a.CompanyName,
                RC = a.RCorNIN,
                Address = a.AddressorLocation,
                City = a.City,
                CompanyContact = a.BusinessContact,
                Owner = a.Owner,
                BusinessLogoUrl = a.BusinessLogoorImageUrl,
                Locations = a.Locations,
                YearsOfService = a.YearsOfService,
                AccountNumber = a.AccountNumber,
                BankName = a.BankName,
                AccountName = a.AccountName,
                ServiceProviderRole = a.ServiceProviderRole.ToString(),
                Price = a.Price
            });
            respomse.Success = true;
            respomse.Message = "Ambulances retrieved successfully";
            respomse.Data = ambulanceDtos;
            return respomse;
        }

        public async Task<Response<ServiceProviderDto>> GetAmbulanceByUserIdAsync(Guid UserId)
        {
            var response = new Response<ServiceProviderDto>();
            var ambulance = await _unitofWork.Ambulance.GetAsync(x => x.UserId == UserId);
            if (ambulance == null)
            {
                response.Success = false;
                response.Message = "Ambulance not found";
                return response;
            }
            var ambulanceDto = new ServiceProviderDto
            {
                Id = ambulance.Id,
                UserId = ambulance.UserId,
                CompanyName = ambulance.CompanyName,
                RC = ambulance.RCorNIN,
                Address = ambulance.AddressorLocation,
                City = ambulance.City,
                CompanyContact = ambulance.BusinessContact,
                Owner = ambulance.Owner,
                BusinessLogoUrl = ambulance.BusinessLogoorImageUrl,
                Locations = ambulance.Locations,
                YearsOfService = ambulance.YearsOfService,
                AccountNumber = ambulance.AccountNumber,
                BankName = ambulance.BankName,
                AccountName = ambulance.AccountName,
                ServiceProviderRole = ambulance.ServiceProviderRole.ToString(),
                Price = ambulance.Price
            };
            response.Success = true;
            response.Message = "Ambulance retrieved successfully";
            response.Data = ambulanceDto;
            return response;
        }

        public async Task<Response<FreelancerDto>> RegisterFreelancer(Guid userId, FreelancerRequest request, IFormFile file)
        {
            var response = new Response<FreelancerDto>();
            var user = await _unitofWork.User.GetAsync(x => x.Id == userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            if (file != null)
            {
                request.ImageUrl = await _unitofWork.FileUpload.UploadAsync(file);
            }

            var ambulance = new AmbulanceCompany
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
                ServiceProviderRole = ServiceProviderRole.Freelancer,
                IsAvailable = true,
            };
            var result = await _unitofWork.Ambulance.AddAsync(ambulance);
            var service = await _unitofWork.Context.ServiceProvidings
                .FirstOrDefaultAsync(x => x.Name == "Ambulance");
            if (service == null)
            {
                service = new ServiceProviding
                {
                    Name = "Ambulance ",
                    Description = "ambulance Service",
                    //ImageUrl = "default_image_url" // Replace with actual default image URL
                };
                await _unitofWork.Context.ServiceProvidings.AddAsync(service);
                await _unitofWork.Context.SaveChangesAsync();
            }
            user.ServiceProviderRole = ambulance.ServiceProviderRole;
            await _unitofWork.User.UpdateAsync(user);

            var userServiceproviding = new UserServiceProviding
            {
                UserId = user.Id,
                ServiceProvidingId = service.Id,
            };
            await _unitofWork.Context.UserServiceProvidings.AddAsync(userServiceproviding);
            await _unitofWork.Context.SaveChangesAsync();
            var emailDto = new EmailDto
            {
                To = user.Email,
                Subject = "Welcome to CaptainTrack",
                Body = $"Hello {user.FullName},<br/>You've registered on CaptainTrack. Pls await verification"
            };
            await _unitofWork.Email.SendEmailAsync(emailDto);
            response.Success = true;
            response.Message = "Freelancer registered successfully";
            response.Data = new FreelancerDto
            {
                Id = result.Id,
                UserId = result.UserId,
                Name = user.FullName,
                Email = user.Email,
                NIN = request.NIN,
                Location = request.Location,
                City = request.City,
                BusinessContact = request.BusinessContact,
                ImageUrl = request.ImageUrl,
                AccountNumber = request.AccountNumber,
                BankName = request.BankName,
                AccountName = request.AccountName,
                ServiceProviderRole = result.ServiceProviderRole.ToString()
            };
            return response;
        }

        public async Task<Response<ServiceProviderDto>> RegisterStoreOwner(Guid userId, ServiceProviderRequest request, IFormFileCollection files)
        {
            var response = new Response<ServiceProviderDto>();
            var user = await _unitofWork.User.GetAsync(x => x.Id == userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                var url = await _unitofWork.FileUpload.UploadAsync(file);

                switch (file.Name)
                {
                    case "BusinessLogoUrl":
                        request.BusinessLogoUrl = url;
                        break;
                }
            }

            var ambulance = new AmbulanceCompany
            {
                UserId = userId,
                CompanyName = request.CompanyName,
                RCorNIN = request.RC,
                AddressorLocation = request.Address,
                City = request.City,
                BusinessContact = request.CompanyContact,
                Owner = request.Owner,
                BusinessLogoorImageUrl = request.BusinessLogoUrl,
                Locations = request.Locations,
                YearsOfService = request.YearsOfService,
                AccountNumber = request.AccountNumber,
                BankName = request.BankName,
                AccountName = request.AccountName,
                ServiceProviderRole = ServiceProviderRole.StoreOwner
            };
            var result = await _unitofWork.Ambulance.AddAsync(ambulance);

            var service = await _unitofWork.Context.ServiceProvidings
                .FirstOrDefaultAsync(x => x.Name == "Ambulance");
            if (service == null)
            {
                service = new ServiceProviding
                {
                    Name = "Ambulance",
                    Description = "Ambulance Service",
                    //ImageUrl = "default_image_url" // Replace with actual default image URL
                };
                await _unitofWork.Context.ServiceProvidings.AddAsync(service);
                await _unitofWork.Context.SaveChangesAsync();
            }
            //user.ServiceProvidings.Add(service);
            user.ServiceProviderRole = ambulance.ServiceProviderRole;
            await _unitofWork.User.UpdateAsync(user);

            var userServiceproviding = new UserServiceProviding
            {
                UserId = user.Id,
                ServiceProvidingId = service.Id,
            };
            await _unitofWork.Context.UserServiceProvidings.AddAsync(userServiceproviding);
            await _unitofWork.Context.SaveChangesAsync();
            var emailDto = new EmailDto
            {
                To = user.Email,
                Subject = "Welcome to CaptainTrack",
                Body = $"Hello {user.FullName},<br/>You've registered on CaptainTrack. Pls await verification"
            };
            await _unitofWork.Email.SendEmailAsync(emailDto);
            response.Success = true;
            response.Message = "Store Owner registered successfully";
            response.Data = new ServiceProviderDto
            {
                Id = result.Id,
                UserId = result.UserId,
                CompanyName = result.CompanyName,
                RC = result.RCorNIN,
                Address = result.AddressorLocation,
                City = result.City,
                CompanyContact = result.BusinessContact,
                Owner = result.Owner,
                BusinessLogoUrl = result.BusinessLogoorImageUrl,
                Locations = result.Locations,
                YearsOfService = result.YearsOfService,
                AccountNumber = result.AccountNumber,
                BankName = result.BankName,
                AccountName = result.AccountName,
                ServiceProviderRole = result.ServiceProviderRole.ToString()
            };
            return response;
        }

        public async Task<Response<bool>> SetPrice(decimal price, Guid? userId = null)
        {
            var response = new Response<bool>();
            if (userId != null)
            {
                var ambulance = await _unitofWork.Ambulance.GetAsync(x => x.UserId == userId);
                if (ambulance == null)
                {
                    response.Success = false;
                    response.Message = "Ambulance not found for the given user ID";
                    return response;
                }
                ambulance.Price = price;
                await _unitofWork.Ambulance.UpdateAsync(ambulance);
                response.Success = true;
                response.Message = "Price set";
                response.Data = true;
                return response;
            }
            var ambprice = await _unitofWork.Context.Prices.FirstOrDefaultAsync();
            if (ambprice == null)
            {
                ambprice = new Price
                {
                    AmbulancePrice = price
                };
                await _unitofWork.Context.Prices.AddAsync(ambprice);
                await _unitofWork.Context.SaveChangesAsync();
                response.Success = true;
                response.Message = "Price set";
                response.Data = true;
                return response;
            }
            ambprice.AmbulancePrice = price;
            _unitofWork.Context.Prices.Update(ambprice);
            await _unitofWork.Context.SaveChangesAsync();
            response.Success = true;
            response.Message = "Price set";
            response.Data = true;
            return response;
        }
    }
}
