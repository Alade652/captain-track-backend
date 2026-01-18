using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.GasDelivery;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.GasDelivery;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.House_Moving;
using CaptainTrackBackend.Domain.Enum;
using CaptainTrackBackend.Domain.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CaptainTrackBackend.Application.Services.ServiceProviders.GasDelivery
{
    public class GasSupplierService : IGasSupplierService
    {
        private readonly IUnitofWork _unitOfWork;
        public GasSupplierService(IUnitofWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<List<ServiceProviderDto>>> GetAllGasSuppliers()
        {
            var gasSuppliers = await _unitOfWork.GasSupplier.GetAllAsync();
            if (gasSuppliers == null)
            {
                return new Response<List<ServiceProviderDto>>
                {
                    Message = "No gas suppliers found",
                    Success = false,
                };
            }
            var gasSuppliersDtos = gasSuppliers.Select(g => new ServiceProviderDto
            {
                Id = g.Id,
                UserId = g.UserId,
                CompanyName = g.CompanyName,
                RC = g.RCorNIN,
                CompanyContact = g.BusinessContact,
                Address = g.AddressorLocation,
                Owner = g.Owner,
                Locations = g.Locations,
                City = g.City,
                YearsOfService = g.YearsOfService,
                BusinessLogoUrl = g.BusinessLogoorImageUrl,
                AccountName = g.AccountName,
                AccountNumber = g.AccountNumber,
                BankName = g.BankName,
                ServiceProviderRole = g.ServiceProviderRole.ToString(),
                Price = g.PricePerKg
            }).ToList();
            return new Response<List<ServiceProviderDto>>
            {
                Message = "Gas suppliers retrieved successfully",
                Success = true,
                Data = gasSuppliersDtos
            };
        }

        public async Task<Response<ServiceProviderDto>> GetGasSupplierByUserId(Guid userId)
        {
            var gasSupplier = await _unitOfWork.GasSupplier.GetAsync(x => x.UserId == userId);
            if (gasSupplier == null)
            {
                return new Response<ServiceProviderDto>
                {
                    Message = "Gas supplier not found",
                    Success = false,
                };
            }
            return new Response<ServiceProviderDto>
            {
                Message = "Gas supplier retrieved successfully",
                Success = true,
                Data = new ServiceProviderDto
                {
                    Id = gasSupplier.Id,
                    UserId = gasSupplier.UserId,
                    CompanyName = gasSupplier.CompanyName,
                    RC = gasSupplier.RCorNIN,
                    CompanyContact = gasSupplier.BusinessContact,
                    Address = gasSupplier.AddressorLocation,
                    Owner = gasSupplier.Owner,
                    Locations = gasSupplier.Locations,
                    City = gasSupplier.City,
                    YearsOfService = gasSupplier.YearsOfService,
                    BusinessLogoUrl = gasSupplier.BusinessLogoorImageUrl,
                    AccountName = gasSupplier.AccountName,
                    AccountNumber = gasSupplier.AccountNumber,
                    BankName = gasSupplier.BankName,
                    ServiceProviderRole = gasSupplier.ServiceProviderRole.ToString(),
                    Price = gasSupplier.PricePerKg
                }
            };
        }

        public async Task<Response<ServiceProviderDto>> Register(Guid userId, ServiceProviderRequest request, IFormFileCollection files)
        {
            var user = await _unitOfWork.User.GetAsync(x => x.Id == userId);
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

                var url = await _unitOfWork.FileUpload.UploadAsync(file);

                switch (file.Name)
                {
                    case "BusinessLogoUrl":
                        request.BusinessLogoUrl = url;
                        break;
                }
            }

            var gasSupplier = new GasSupplier
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
            await _unitOfWork.GasSupplier.AddAsync(gasSupplier);

            var service = await _unitOfWork.Context.ServiceProvidings
                .FirstOrDefaultAsync(x => x.Name == "Gas Delivery");
            if (service == null)
            {
                service = new ServiceProviding
                {
                    Name = "Gas Delivery",
                    Description = "Gas Delivery Service",
                    //ImageUrl = "default_image_url" // Replace with actual default image URL
                };
                await _unitOfWork.Context.ServiceProvidings.AddAsync(service);
                await _unitOfWork.Context.SaveChangesAsync();
            }
            //user.ServiceProvidings.Add(service);
            user.ServiceProviderRole = gasSupplier.ServiceProviderRole;
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

            return new Response<ServiceProviderDto>
            {
                Message = "Registration Succesful",
                Success = true,
                Data = new ServiceProviderDto
                {
                    UserId = user.Id,
                    Id = gasSupplier.Id,
                    CompanyName = gasSupplier.CompanyName,
                    City = gasSupplier.City,
                    RC = gasSupplier.RCorNIN,
                    CompanyContact = gasSupplier.BusinessContact,
                    Address = gasSupplier.AddressorLocation,
                    Owner = gasSupplier.Owner,
                    Locations = gasSupplier.Locations,
                    YearsOfService = gasSupplier.YearsOfService,
                    BusinessLogoUrl = gasSupplier.BusinessLogoorImageUrl,
                    AccountName = request.AccountName,
                    AccountNumber = request.AccountNumber,
                    BankName = request.BankName,
                    ServiceProviderRole = gasSupplier.ServiceProviderRole.ToString(),
                }
            };
        }

        public async Task<Response<FreelancerDto>> RegisterFreelancer(Guid userId, FreelancerRequest request, IFormFile file)
        {
            var user =await _unitOfWork.User.GetAsync(x => x.Id == userId);
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
                request.ImageUrl = await _unitOfWork.FileUpload.UploadAsync(file);
            }

            var gasSupplier = new GasSupplier
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
            await _unitOfWork.GasSupplier.AddAsync(gasSupplier);

            var service = await _unitOfWork.Context.ServiceProvidings
                .FirstOrDefaultAsync(x => x.Name == "Gas Delivery");
            if (service == null)
            {
                service = new ServiceProviding
                {
                    Name = "Gas Delivery",
                    Description = "Gas Delivery Service",
                    //ImageUrl = "default_image_url" // Replace with actual default image URL
                };
                await _unitOfWork.Context.ServiceProvidings.AddAsync(service);
                await _unitOfWork.Context.SaveChangesAsync();
            }
            //user.ServiceProvidings.Add(service);
            user.ServiceProviderRole = gasSupplier.ServiceProviderRole;
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
            return new Response<FreelancerDto>
            {
                Message = "Registration Succesful",
                Success = true,
                Data = new FreelancerDto
                {
                    UserId = user.Id,
                    Id = gasSupplier.Id,
                    NIN = gasSupplier.RCorNIN,
                    BusinessContact = gasSupplier.BusinessContact,
                    Location = gasSupplier.AddressorLocation,
                    City = gasSupplier.City,
                    ImageUrl = gasSupplier.BusinessLogoorImageUrl,
                    AccountName = request.AccountName,
                    AccountNumber = request.AccountNumber,
                    BankName = request.BankName,
                    ServiceProviderRole = gasSupplier.ServiceProviderRole.ToString(),
                }
            };
        }

        public async Task<Response<string>> SetPrice(decimal pricePerKg, Guid gasSupplierUserId)
        {
            var gasSupplier = await _unitOfWork.GasSupplier.GetAsync(x => x.UserId == gasSupplierUserId);
            if (gasSupplier == null)
            {
                return new Response<string>
                {
                    Message = "Gas supplier not found",
                    Success = false,
                };
            }
            gasSupplier.PricePerKg = pricePerKg;
            await _unitOfWork.GasSupplier.UpdateAsync(gasSupplier);
            return new Response<string>
            {
                Message = "Price per kg set successfully",
                Success = true,
                Data = pricePerKg.ToString()
            };
        }

        public Task<Response<string>> SetPricePerKg(decimal pricePerKg)
        {
            var price = _unitOfWork.Context.Prices.FirstOrDefault();

            price.GasPriceperKg = pricePerKg;
            _unitOfWork.Context.Prices.Update(price);
            _unitOfWork.Context.SaveChanges();
            return Task.FromResult(new Response<string>
            {
                Message = "Price per kg set successfully",
                Success = true,
                Data = pricePerKg.ToString()
            });
        }

/*        public async Task<Response<bool>> Update(Guid gasSupplierUserId)
        {
            var gasSupplier = await _unitOfWork.GasSupplier.GetAsync(x => x.UserId == gasSupplierUserId);
            var user = await _unitOfWork.User.GetAsync(x => x.Id == gasSupplierUserId);

            user.Service = ServiceProviding.GasDelivery;
            user.ServiceProviderRole = ServiceProviderRole.Freelancer;
            await _unitOfWork.User.UpdateAsync(user);
            return new Response<bool>
            {
                Message = "Gas supplier updated successfully",
                Success = true,
                Data = true
            };
        }*/

        /* public async Task<Response<ServiceProviderDto>> Update(Guid gasSupplierUserId, GasSupplierUpdateRequest request, IFormFileCollection files)
         {
             var gasSupplier = await _unitOfWork.GasSupplier.GetAsync(x => x.UserId == gasSupplierUserId);
             if (gasSupplier == null)
             {
                 return new Response<ServiceProviderDto>
                 {
                     Message = "Gas supplier not found",
                     Success = false,
                 };
             }

             foreach (var file in files)
             {
                 if (file.Length == 0) continue;

                 var url = await _unitOfWork.FileUpload.UploadAsync(file);

                 switch (file.Name)
                 {
                     case "BusinessLogo":
                         request.BusinessLogo = url;
                         break;
                 }
             }

             gasSupplier.CompanyName = request.CompanyName ?? gasSupplier.CompanyName;
             gasSupplier.RCorNIN = request.RC ?? gasSupplier.RCorNIN;
             gasSupplier.AddressorLocation = request.AddressorLocation ?? gasSupplier.AddressorLocation;
             gasSupplier.City = request.City ?? gasSupplier.City;
             gasSupplier.BusinessContact = request.BusinessContact ?? gasSupplier.BusinessContact;
             gasSupplier.PricePerKg = request.PricePerKg > 0 ? request.PricePerKg : gasSupplier.PricePerKg;
             gasSupplier.Owner = request.Owner ?? gasSupplier.Owner;
             gasSupplier.BusinessLogoorImageUrl = request.BusinessLogo ?? gasSupplier.BusinessLogoorImageUrl;
             gasSupplier.Locations = request.Locations > 0 ? request.Locations : gasSupplier.Locations;
             gasSupplier.YearsOfService = request.YearsOfService ?? gasSupplier.YearsOfService;
             gasSupplier.AccountName = request.AccountName ?? gasSupplier.AccountName;
             gasSupplier.AccountNumber = request.AccountNumber ?? gasSupplier.AccountNumber;
             gasSupplier.BankName = request.BankName ?? gasSupplier.BankName;
             await _unitOfWork.GasSupplier.UpdateAsync(gasSupplier);

             return new Response<ServiceProviderDto>
             {
                 Message = "Gas supplier updated successfully",
                 Success = true,
                 Data = new ServiceProviderDto
                 {
                     Id = gasSupplier.Id,
                     UserId = gasSupplier.UserId,
                     CompanyName = gasSupplier.CompanyName,
                     RC = gasSupplier.RCorNIN,
                     Address = gasSupplier.AddressorLocation,
                     PricePerKg = gasSupplier.PricePerKg,
                     City = gasSupplier.City,
                     CompanyContact = gasSupplier.BusinessContact,
                     Owner = gasSupplier.Owner,
                     BusinessLogoorImageUrl = gasSupplier.BusinessLogoorImageUrl,
                     Locations = gasSupplier.Locations,
                     YearsOfService = gasSupplier.YearsOfService,
                     AccountName = gasSupplier.AccountName,
                     AccountNumber = gasSupplier.AccountNumber,
                     BankName = gasSupplier.BankName,
                     ServiceProviding = gasSupplier.ServiceProviding.ToString(),
                     ServiceProviderRole = gasSupplier.ServiceProviderRole.ToString(),
                 }
             };
         }*/
    }
}
