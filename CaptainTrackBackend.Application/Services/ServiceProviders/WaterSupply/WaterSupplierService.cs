using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.WaterSupply;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.GasDelivery;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.WaterSupply;
using CaptainTrackBackend.Domain.Enum;
using CaptainTrackBackend.Domain.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Services.ServiceProviders.WaterSupply
{
    public class WaterSupplierService : IWaterSupplierService
    {
        private readonly IUnitofWork _unitOfWork;

        public WaterSupplierService(IUnitofWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<ServiceProviderDto>> GetWaterSupplier(Guid userId)
        {
            var response = new Response<ServiceProviderDto>();
            var waterSuppplier = await _unitOfWork.WaterSupplier.GetAsync(x => x.UserId == userId);
            if (waterSuppplier == null)
            {
                response.Success = false;
                response.Message = "Water supplier not found.";
                return response;
            }

            response.Data = new ServiceProviderDto
            {
                UserId = waterSuppplier.UserId,
                Id = waterSuppplier.Id,
                CompanyName = waterSuppplier.CompanyName,
                City = waterSuppplier.City,
                RC = waterSuppplier.RCorNIN,
                CompanyContact = waterSuppplier.BusinessContact,
                Address = waterSuppplier.AddressorLocation,
                Owner = waterSuppplier.Owner,
                Locations = waterSuppplier.Locations,
                YearsOfService = waterSuppplier.YearsOfService,
                BusinessLogoUrl = waterSuppplier.BusinessLogoorImageUrl,
                AccountName = waterSuppplier.AccountName,
                AccountNumber = waterSuppplier.AccountNumber,
                BankName = waterSuppplier.BankName,
                ServiceProviderRole = waterSuppplier.ServiceProviderRole.ToString(),
                Price = waterSuppplier.PricePerLitre
            };
            response.Message = "Water supplier retrieved successfully.";
            response.Success = true;
            return response;
        }

        public async Task<Response<IEnumerable<ServiceProviderDto>>> GetWaterSuppliers()
        {
            var response = new Response<IEnumerable<ServiceProviderDto>>();
            var waterSuppliers = await _unitOfWork.WaterSupplier.GetAllByExpression(w => w.ServiceProviderRole == ServiceProviderRole.StoreOwner);
            if (waterSuppliers == null || !waterSuppliers.Any())
            {
                response.Success = false;
                response.Message = "No water suppliers found.";
                return response;
            }

            response.Data = waterSuppliers.Select(w => new ServiceProviderDto
            {
                UserId = w.UserId,
                Id = w.Id,
                CompanyName = w.CompanyName,
                City = w.City,
                RC = w.RCorNIN,
                CompanyContact = w.BusinessContact,
                Address = w.AddressorLocation,
                Owner = w.Owner,
                Locations = w.Locations,
                YearsOfService = w.YearsOfService,
                BusinessLogoUrl = w.BusinessLogoorImageUrl,
                AccountName = w.AccountName,
                AccountNumber = w.AccountNumber,
                BankName = w.BankName,
                ServiceProviderRole = w.ServiceProviderRole.ToString(),
                Price = w.PricePerLitre
            });
            response.Message = "Water suppliers retrieved successfully.";
            response.Success = true;
            return response;
            throw new NotImplementedException();
        }

        public async Task<Response<FreelancerDto>> RegisterFreelancer(Guid userId, FreelancerRequest request, IFormFile file)
        {
            var response = new Response<FreelancerDto>();
            var user = await _unitOfWork.User.GetAsync(userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }

            if (file != null)
            {
                request.ImageUrl = await _unitOfWork.FileUpload.UploadAsync(file);
            }

            var waterSupplier = new WaterSupplier
            {
                UserId = userId,
                RCorNIN = request.NIN,
                BusinessContact = request.BusinessContact,
                AddressorLocation = request.Location,
                City = request.City,
                BusinessLogoorImageUrl = request.ImageUrl,
                AccountName = request.AccountName,
                AccountNumber = request.AccountNumber,
                BankName = request.BankName,
                ServiceProviderRole = ServiceProviderRole.Freelancer,
            };
            await _unitOfWork.WaterSupplier.AddAsync(waterSupplier);

            var service = await _unitOfWork.Context.ServiceProvidings
                .FirstOrDefaultAsync(x => x.Name == "Water Supply");
            if (service == null)
            {
                service = new ServiceProviding
                {
                    Name = "Water Supply",
                    Description = "Water Supply Service",
                };
                await _unitOfWork.Context.ServiceProvidings.AddAsync(service);
                await _unitOfWork.Context.SaveChangesAsync();
            }
            //user.ServiceProvidings.Add(service);
            user.ServiceProviderRole = waterSupplier.ServiceProviderRole;
            await _unitOfWork.User.UpdateAsync(user);

            var userServiceproviding = new UserServiceProviding
            {
                UserId = user.Id,
                ServiceProvidingId = service.Id,
            };
            await _unitOfWork.Context.UserServiceProvidings.AddAsync(userServiceproviding);
            await _unitOfWork.Context.SaveChangesAsync();
            var emailDto = new EmailDto
            {
                To = user.Email,
                Subject = "Welcome to CaptainTrack",
                Body = $"Hello {user.FullName},<br/>You've registered on CaptainTrack. Pls await verification"
            };
            await _unitOfWork.Email.SendEmailAsync(emailDto);


            response.Message = "Registration Succesful";
            response.Success = true;
            response.Data = new FreelancerDto
            {
                UserId = user.Id,
                Id = waterSupplier.Id,
                NIN = waterSupplier.RCorNIN,
                BusinessContact = waterSupplier.BusinessContact,
                Location = waterSupplier.AddressorLocation,
                City = waterSupplier.City,
                ImageUrl = waterSupplier.BusinessLogoorImageUrl,
                AccountName = waterSupplier.AccountName,
                AccountNumber = waterSupplier.AccountNumber,
                BankName = waterSupplier.BankName,
                ServiceProviderRole = waterSupplier.ServiceProviderRole.ToString(),
            };
            return response;
        }

        public async Task<Response<ServiceProviderDto>> RegisterStoreOwner(Guid userId, ServiceProviderRequest request, IFormFileCollection files)
        {
            var response = new Response<ServiceProviderDto>();
            var user = await _unitOfWork.User.GetAsync(userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }

            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                var url = await _unitOfWork.FileUpload.UploadAsync(file);

                switch (file.Name)
                {
                    case "BusinessLogoUrl":
                        request.BusinessLogoUrl = url;
                        break;
                }
            }

            var waterSupplier = new WaterSupplier
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
            await _unitOfWork.WaterSupplier.AddAsync(waterSupplier);

            var service = await _unitOfWork.Context.ServiceProvidings
                .FirstOrDefaultAsync(x => x.Name == "Water Supply");
            if (service == null)
            {
                service = new ServiceProviding
                {
                    Name = "Water Supply",
                    Description = "Water Supply Service",
                    //ImageUrl = "default_image_url" // Replace with actual default image URL
                };
                await _unitOfWork.Context.ServiceProvidings.AddAsync(service);
                await _unitOfWork.Context.SaveChangesAsync();
            }
            //user.ServiceProvidings.Add(service);
            user.ServiceProviderRole = waterSupplier.ServiceProviderRole;
            await _unitOfWork.User.UpdateAsync(user);

            var userServiceproviding = new UserServiceProviding
            {
                UserId = user.Id,
                ServiceProvidingId = service.Id,
            };
            await _unitOfWork.Context.UserServiceProvidings.AddAsync(userServiceproviding);
            await _unitOfWork.Context.SaveChangesAsync();
            var emailDto = new EmailDto
            {
                To = user.Email,
                Subject = "Welcome to CaptainTrack",
                Body = $"Hello {user.FullName},<br/>You've registered on CaptainTrack. Pls await verification"
            };
            await _unitOfWork.Email.SendEmailAsync(emailDto);


            response.Message = "Registration Succesful";
            response.Success = true;
            response.Data = new ServiceProviderDto
            {
                UserId = user.Id,
                Id = waterSupplier.Id,
                CompanyName = waterSupplier.CompanyName,
                City = waterSupplier.City,
                RC = waterSupplier.RCorNIN,
                CompanyContact = waterSupplier.BusinessContact,
                Address = waterSupplier.AddressorLocation,
                Owner = waterSupplier.Owner,
                Locations = waterSupplier.Locations,
                YearsOfService = waterSupplier.YearsOfService,
                BusinessLogoUrl = waterSupplier.BusinessLogoorImageUrl,
                AccountName = request.AccountName,
                AccountNumber = request.AccountNumber,
                BankName = request.BankName,
                ServiceProviderRole = waterSupplier.ServiceProviderRole.ToString(),
            };
            return response;
        }

        public async Task<Response<decimal>> SetPrice(decimal priceperLiter, Guid? userId = null)
        {
            var response = new Response<decimal>();
            if (userId != null)
            {
                var ws = await _unitOfWork.WaterSupplier.GetAsync(x => x.UserId == userId);
                if (ws == null)
                {
                    response.Success = false;
                    response.Message = "Water supplier not found.";
                    return response;
                }
                ws.PricePerLitre = priceperLiter;
                await _unitOfWork.WaterSupplier.UpdateAsync(ws);
            }
            else
            {
                var price = await _unitOfWork.Context.Prices.FirstOrDefaultAsync();
                price.WaterPricePerLitre = priceperLiter;
                _unitOfWork.Context.Prices.Update(price);
                await _unitOfWork.Context.SaveChangesAsync();
            }
            response.Data = priceperLiter;
            response.Success = true;
            response.Message = "Price set successfully.";
            return response;
        }
    }
}
