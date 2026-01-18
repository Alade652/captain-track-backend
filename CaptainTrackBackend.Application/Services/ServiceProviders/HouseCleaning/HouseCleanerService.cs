using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.HouseCleaning;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.HouseCleaning;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.House_Moving;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.HouseCleaning;
using CaptainTrackBackend.Domain.Enum;
using CaptainTrackBackend.Domain.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CaptainTrackBackend.Application.Services.ServiceProviders.HouseCleaning
{
    public class HouseCleanerService : IHouseCleanerService
    {
        private readonly IUnitofWork _unitofWork;

        public HouseCleanerService(IUnitofWork unitofWork)
        {
            _unitofWork = unitofWork;
        }

        public async Task<Response<ServiceProviderDto>> RegisterStoreOwner(Guid userId, ServiceProviderRequest request, IFormFileCollection files)
        {
            var response = new Response<ServiceProviderDto>();
            var user = await _unitofWork.User.GetAsync(userId);
            if (user == null)
            {
                response.Message = "user not found";
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

            var houseCleaner = new HouseCleaner
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
            await _unitofWork.HouseCleaner.AddAsync(houseCleaner);
            var service = await _unitofWork.Context.ServiceProvidings
                .FirstOrDefaultAsync(x => x.Name == "House Cleaning");
            if (service == null)
            {
                service = new ServiceProviding
                {
                    Name = "House Cleaning",
                    Description = "House Cleaning Service",
                    //ImageUrl = "default_image_url" // Replace with actual default image URL
                };
                await _unitofWork.Context.ServiceProvidings.AddAsync(service);
                await _unitofWork.Context.SaveChangesAsync();
            }
            //user.ServiceProvidings.Add(service);
            user.ServiceProviderRole = houseCleaner.ServiceProviderRole;
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

            return new Response<ServiceProviderDto>
            {
                Message = "Registration Succesful",
                Success = true,
                Data = new ServiceProviderDto
                {
                    UserId = user.Id,
                    Id = houseCleaner.Id,
                    CompanyName = houseCleaner.CompanyName,
                    City = houseCleaner.City,
                    RC = houseCleaner.RCorNIN,
                    CompanyContact = houseCleaner.BusinessContact,
                    Address = houseCleaner.AddressorLocation,
                    Owner = houseCleaner.Owner,
                    Locations = houseCleaner.Locations,
                    YearsOfService = houseCleaner.YearsOfService,
                    BusinessLogoUrl = houseCleaner.BusinessLogoorImageUrl,
                    AccountName = request.AccountName,
                    AccountNumber = request.AccountNumber,
                    BankName = request.BankName,
                    ServiceProviderRole = houseCleaner.ServiceProviderRole.ToString(),

                }
            };
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
            var houseCleaner = new HouseCleaner
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
            await _unitofWork.HouseCleaner.AddAsync(houseCleaner);

            var service = await _unitofWork.Context.ServiceProvidings
                 .FirstOrDefaultAsync(x => x.Name == "House Cleaning");
            if (service == null)
            {
                service = new ServiceProviding
                {
                    Name = "House Cleaning",
                    Description = "House Cleaning Service",
                    //ImageUrl = "default_image_url" // Replace with actual default image URL
                };
                await _unitofWork.Context.ServiceProvidings.AddAsync(service);
                await _unitofWork.Context.SaveChangesAsync();
            }
            //user.ServiceProvidings.Add(service);
            user.ServiceProviderRole = houseCleaner.ServiceProviderRole;
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

            return new Response<FreelancerDto>
            {
                Message = "Registered Successfully",
                Success = true,
                Data = new FreelancerDto
                {
                    Id = houseCleaner.Id,
                    UserId = user.Id,
                    Name = user.FullName,
                    Email = user.Email,
                    BusinessContact = houseCleaner.BusinessContact,
                    NIN = houseCleaner.RCorNIN,
                    Location = houseCleaner.AddressorLocation,
                    City = houseCleaner.City,
                    BankName = houseCleaner.BankName,
                    AccountName = houseCleaner.AccountName,
                    AccountNumber = houseCleaner.AccountNumber,
                    ImageUrl = houseCleaner.BusinessLogoorImageUrl,
                    ServiceProviderRole = houseCleaner.ServiceProviderRole.ToString()
                }
            };
        }

        public async Task<Response<List<ServiceProviderDto>>> GetHouseCleaners()
        {
            var response = new Response<List<ServiceProviderDto>>();
            var houseCleaners = await _unitofWork.HouseCleaner.GetAllAsync();
            if (houseCleaners == null || !houseCleaners.Any())
            {
                response.Message = "No house cleaners found";
                response.Success = false;
                return response;
            }

            response.Data = houseCleaners.Select(hc => new ServiceProviderDto
            {
                Id = hc.Id,
                UserId = hc.UserId,
                CompanyName = hc.CompanyName,
                City = hc.City,
                RC = hc.RCorNIN,
                CompanyContact = hc.BusinessContact,
                Address = hc.AddressorLocation,
                Owner = hc.Owner,
                Locations = hc.Locations,
                YearsOfService = hc.YearsOfService,
                BusinessLogoUrl = hc.BusinessLogoorImageUrl,
                AccountName = hc.AccountName,
                AccountNumber = hc.AccountNumber,
                BankName = hc.BankName,
                ServiceProviderRole = hc.ServiceProviderRole.ToString()
            }).ToList();
            response.Message = "House cleaners retrieved successfully";
            response.Success = true;
            return response;
        }

        public async Task<Response<HouseCleanerPackageDto>> AddPackage(HouseCleanerPackageRequest request, Guid? houseCleanerUserId = null)
        {
            var response = new Response<HouseCleanerPackageDto>();
            if (houseCleanerUserId != null)
            {
                var houseCleaner = await _unitofWork.HouseCleaner.GetAsync(x => x.UserId == houseCleanerUserId);
                if (houseCleaner == null)
                {
                    response.Message = "House cleaner not found";
                    return response;
                }
                var houseCleanerPackage = new HouseCleanerPackage   
                {
                    HouseCleanerId = houseCleaner.Id,
                    Name = request.Name,
                    Description = request.Description,
                    ImageUrl = request.ImageUrl != null ? await _unitofWork.FileUpload.UploadAsync(request.ImageUrl) : null
                };
                await _unitofWork.Context.HouseCleanerPackages.AddAsync(houseCleanerPackage);
                await _unitofWork.Context.SaveChangesAsync();
                response.Data = new HouseCleanerPackageDto
                {
                    HouseCleanerPackageId = houseCleanerPackage.Id,
                    HouseCleanerId = houseCleanerPackage.HouseCleanerId,
                    Name = houseCleanerPackage.Name,
                    Description = houseCleanerPackage.Description,
                    ImageUrl = houseCleanerPackage.ImageUrl
                };
                response.Message = "Package added successfully";
                response.Success = true;
                return response;
            }

            var package = new HouseCleanerPackage
            {
                Name = request.Name,
                Description = request.Description,
                ImageUrl = request.ImageUrl != null ? await _unitofWork.FileUpload.UploadAsync(request.ImageUrl) : null
            };
            await _unitofWork.Context.HouseCleanerPackages.AddAsync(package);
            await _unitofWork.Context.SaveChangesAsync();
            response.Data = new HouseCleanerPackageDto
            {
                HouseCleanerPackageId = package.Id,
                Name = package.Name,
                Description = package.Description,
                ImageUrl = package.ImageUrl
            };
            response.Message = "Package added successfully";
            response.Success = true;
            return response;
        }

        public async Task<Response<List<HouseCleanerPackageDto>>> GetPackages(Guid? houseCleanerUserId)
        {
            var response = new Response<List<HouseCleanerPackageDto>>();
            if (houseCleanerUserId != null)
            {
                var houseCleaner = await _unitofWork.HouseCleaner.GetAsync(x => x.UserId == houseCleanerUserId);
                if (houseCleaner == null)
                {
                    response.Message = "House cleaner not found";
                    return response;
                }
                var packages = await _unitofWork.Context.HouseCleanerPackages.Include(p => p.HouseCleanerItems)
                    .Where(x => x.HouseCleanerId == houseCleaner.Id)
                    .ToListAsync();
                response.Data = packages.Select(p => new HouseCleanerPackageDto
                {
                    HouseCleanerPackageId = p.Id,
                    HouseCleanerId = p.HouseCleanerId,
                    Name = p.Name,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl,
                    HouseCleanerItems = p.HouseCleanerItems.Select(item => new HouseCleanerItemDto
                    {
                        HouseCleanerItemId = item.Id,
                        Name = item.Name,
                        Price = item.Price
                    }).ToList()
                }).ToList();
            }
            else
            {
                var packages = await _unitofWork.Context.HouseCleanerPackages.Include(x => x.HouseCleanerItems).Where(x => x.HouseCleanerId == null).ToListAsync();
                response.Data = packages.Select(p => new HouseCleanerPackageDto
                {
                    HouseCleanerPackageId = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl,
                    HouseCleanerItems = p.HouseCleanerItems.Select(item => new HouseCleanerItemDto
                    {
                        HouseCleanerItemId = item.Id,
                        Name = item.Name,
                        Price = item.Price
                    }).ToList()
                }).ToList();
            }
            response.Message = "Packages retrieved successfully";
            response.Success = true;
            return response;
        }

        public async Task<Response<HouseCleanerItemDto>> AddItem(Guid houseCleanerPackageId, HouseCleanerItemRequest request)
        {
            var response = new Response<HouseCleanerItemDto>();
            var package = await _unitofWork.Context.HouseCleanerPackages.FindAsync(houseCleanerPackageId);
            if (package == null)
            {
                response.Message = "Package not found";
                response.Success = false;
                return response;
            }
            var item = new HouseCleanerItem
            {
                HouseCleanerPackageId = houseCleanerPackageId,
                Name = request.Name,
                Price = request.Price
            };
            await _unitofWork.Context.HouseCleanerItems.AddAsync(item);
            await _unitofWork.Context.SaveChangesAsync();
            response.Data = new HouseCleanerItemDto
            {
                HouseCleanerItemId = item.Id,
                HouseCleanerPackageId = item.HouseCleanerPackageId,
                Name = item.Name,
                Price = item.Price
            };
            response.Message = "Item added successfully";
            response.Success = true;
            return response;
        }

        public async Task<Response<List<HouseCleanerItemDto>>> GetItems(Guid houseCleanerPackageId)
        {
            var response = new Response<List<HouseCleanerItemDto>>();
            var items = await _unitofWork.Context.HouseCleanerItems
                .Where(x => x.HouseCleanerPackageId == houseCleanerPackageId)
                .ToListAsync();
            if (items == null || !items.Any())
            {
                response.Message = "No items found for this package";
                response.Success = false;
                return response;
            }
            response.Data = items.Select(i => new HouseCleanerItemDto
            {
                HouseCleanerItemId = i.Id,
                HouseCleanerPackageId = i.HouseCleanerPackageId,
                Name = i.Name,
                Price = i.Price
            }).ToList();
            response.Message = "Items retrieved successfully";
            response.Success = true;
            return response;
        }

        public async Task<Response<HouseCleanerItemDto>> UpdateItemPrice(Guid houseCleanerItemId, decimal price)
        {
            var response = new Response<HouseCleanerItemDto>();
            var item = await _unitofWork.Context.HouseCleanerItems.FindAsync(houseCleanerItemId);
            if (item == null)
            {
                response.Message = "Item not found";
                response.Success = false;
                return response;
            }
            item.Price = price;
            _unitofWork.Context.HouseCleanerItems.Update(item);
            await _unitofWork.Context.SaveChangesAsync();
            response.Data = new HouseCleanerItemDto
            {
                HouseCleanerItemId = item.Id,
                HouseCleanerPackageId = item.HouseCleanerPackageId,
                Name = item.Name,
                Price = item.Price
            };
            response.Message = "Item price updated successfully";
            response.Success = true;
            return response;
        }

        public async Task<Response<ServiceProviderDto>> GetHouseCleaner(Guid userId)
        {
            var response = new Response<ServiceProviderDto>();
            var hc = await _unitofWork.HouseCleaner.GetAsync(x => x.UserId == userId);
            if (hc == null)
            {
                response.Message = "Not found";
                response.Success = false;
                return response;
            }

            response.Data = new ServiceProviderDto
            {
                Id = hc.Id,
                UserId = hc.UserId,
                CompanyName = hc.CompanyName,
                City = hc.City,
                RC = hc.RCorNIN,
                CompanyContact = hc.BusinessContact,
                Address = hc.AddressorLocation,
                Owner = hc.Owner,
                Locations = hc.Locations,
                YearsOfService = hc.YearsOfService,
                BusinessLogoUrl = hc.BusinessLogoorImageUrl,
                AccountName = hc.AccountName,
                AccountNumber = hc.AccountNumber,
                BankName = hc.BankName,
                ServiceProviderRole = hc.ServiceProviderRole.ToString()
            };
            response.Message = "House Cleaner details";
            response.Success = true;
            return response;
        }
    }
}
