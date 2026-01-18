using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.House_Moving;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.House_Moving;
using CaptainTrackBackend.Application.DTO.ServiceProviders.HouseCleaning;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.Ambulance;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.House_Moving;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.HouseCleaning;
using CaptainTrackBackend.Domain.Enum;
using CaptainTrackBackend.Domain.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Services.ServiceProviders.House_Moving
{
    public class HouseMoverService : IHouseMoverService
    {
        private readonly IUnitofWork _unitofWork;
        public HouseMoverService(IUnitofWork unitofWork)
        {
            _unitofWork = unitofWork;
        }

        public async Task<Response<HouseMoverPackageDto>> AddPackage(HouseMoverPackageRequest request, Guid? houseMoverUserId = null)
        {
            var response = new Response<HouseMoverPackageDto>();
            if (houseMoverUserId != null)
            {
                var houseMover = await _unitofWork.HouseMover.GetAsync(x => x.UserId == houseMoverUserId);
                if (houseMover == null)
                {
                    response.Message = "House cleaner not found";
                    return response;
                }
                var houseMoverPackage = new HouseMovingPackage
                {
                    HouseMoverId = houseMover.Id,
                    Name = request.Name,
                    Description = request.Description,
                    ImageUrl = request.Image != null ? await _unitofWork.FileUpload.UploadAsync(request.Image) : null
                };
                await _unitofWork.Context.HouseMovingPackages.AddAsync(houseMoverPackage);
                await _unitofWork.Context.SaveChangesAsync();
                response.Data = new HouseMoverPackageDto
                {
                    PackageId = houseMoverPackage.Id,
                    HouseMoverId = houseMoverPackage.HouseMoverId,
                    Name = houseMoverPackage.Name,
                    Description = houseMoverPackage.Description,
                    ImageUrl = houseMoverPackage.ImageUrl
                };
                response.Message = "Package added successfully";
                response.Success = true;
                return response;
            }

            var package = new HouseMovingPackage
            {
                Name = request.Name,
                Description = request.Description,
                ImageUrl = request.Image != null ? await _unitofWork.FileUpload.UploadAsync(request.Image) : null
            };
            await _unitofWork.Context.HouseMovingPackages.AddAsync(package);
            await _unitofWork.Context.SaveChangesAsync();
            response.Data = new HouseMoverPackageDto
            {
                PackageId = package.Id,
                Name = package.Name,
                Description = package.Description,
                ImageUrl = package.ImageUrl
            };
            response.Message = "Package added successfully";
            response.Success = true;
            return response;
            throw new NotImplementedException();
        }

        public async Task<Response<HouseMovingTruckDto>> AddTruck(HouseMovingTruckRequest request, Guid? houseMoverUserId = null)
        {
            var response = new Response<HouseMovingTruckDto>(); 
            if (houseMoverUserId != null)
            {
                var houseMover = await _unitofWork.HouseMover.GetAsync(x => x.UserId == houseMoverUserId);
                if (houseMover == null)
                {
                    response.Message = "House cleaner not found";
                    return response;
                }
                var houseMovingTruck = new HouseMovingTruck
                {
                    HouseMoverId = houseMover.Id,
                    Name = request.Name,
                    Price = request.Price,
                    ImageUrl = request.Image != null ? await _unitofWork.FileUpload.UploadAsync(request.Image) : null
                };
                await _unitofWork.Context.HouseMovingTrucks.AddAsync(houseMovingTruck);
                await _unitofWork.Context.SaveChangesAsync();
                response.Data = new HouseMovingTruckDto
                {
                    TruckId = houseMovingTruck.Id,
                    HouseMoverId = houseMovingTruck.HouseMoverId,
                    Name = houseMovingTruck.Name,
                    Description = houseMovingTruck.Description,
                    Price = houseMovingTruck.Price,
                    ImageUrl = houseMovingTruck.ImageUrl
                };
                response.Message = "Package added successfully";
                response.Success = true;
                return response;
            }

            var truck = new HouseMovingTruck
            {
                Name = request.Name,
                Price = request.Price,
                ImageUrl = request.Image != null ? await _unitofWork.FileUpload.UploadAsync(request.Image) : null
            };
            await _unitofWork.Context.HouseMovingTrucks.AddAsync(truck);
            await _unitofWork.Context.SaveChangesAsync();
            response.Data = new HouseMovingTruckDto
            {
                TruckId = truck.Id,
                HouseMoverId = truck.Id,
                Name = truck.Name,
                Description = truck.Description,
                Price = truck.Price,
                ImageUrl = truck.ImageUrl
            };
            response.Message = "Package added successfully";
            response.Success = true;
            return response;
        }

        public async Task<Response<ServiceProviderDto>> GetHouseMover(Guid userId)
        {
            var response = new Response<ServiceProviderDto>();
            var houseMover = await _unitofWork.HouseMover.GetAsync(x =>  x.UserId == userId);
            if (houseMover == null)
            {
                response.Success = false;
                response.Message = "Not found";
                return response;
            }

            response.Success = true;
            response.Message = "House Mover Details";
            response.Data = new ServiceProviderDto
            {
                Id = houseMover.Id,
                UserId = houseMover.UserId,
                CompanyName = houseMover.CompanyName,
                RC = houseMover.RCorNIN,
                Address = houseMover.AddressorLocation,
                City = houseMover.City,
                CompanyContact = houseMover.BusinessContact,
                Owner = houseMover.Owner,
                BusinessLogoUrl = houseMover.BusinessLogoorImageUrl,
                Locations = houseMover.Locations,
                YearsOfService = houseMover.YearsOfService,
                AccountNumber = houseMover.AccountNumber,
                BankName = houseMover.BankName,
                AccountName = houseMover.AccountName,
                ServiceProviderRole = houseMover.ServiceProviderRole.ToString(),
            };
            return response;
        }

        public async Task<Response<IEnumerable<ServiceProviderDto>>> GetHouseMovers()
        {
            var response = new Response<IEnumerable<ServiceProviderDto>>();
            var houseMovers = await _unitofWork.HouseMover.GetAllAsync();
            if (houseMovers == null || !houseMovers.Any())
            {
                response.Success = false;
                response.Message = "No houseMovers found";
                return response;
            }
            var ambulanceDtos = houseMovers.Select(a => new ServiceProviderDto
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
            });
            response.Success = true;
            response.Message = "Ambulances retrieved successfully";
            response.Data = ambulanceDtos;
            return response;
        }

        public async Task<Response<IEnumerable<HouseMoverPackageDto>>> GetPackages(Guid? userId = null)
        {
            var response = new Response<IEnumerable<HouseMoverPackageDto>>();
            if (userId != null)
            {
                var houseMover = await _unitofWork.HouseMover.GetAsync(x => x.UserId == userId);
                if (houseMover == null)
                {
                    response.Message = "House cleaner not found";
                    return response;
                }
                var packages = await _unitofWork.Context.HouseMovingPackages
                    .Where(x => x.HouseMoverId == houseMover.Id)
                    .ToListAsync();
                response.Data = packages.Select(p => new HouseMoverPackageDto
                {
                    PackageId = p.Id,
                    HouseMoverId = p.HouseMoverId,
                    Name = p.Name,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl,
                });
            }
            else
            {
                var packages = await _unitofWork.Context.HouseMovingPackages.Where(x => x.HouseMoverId == null).ToListAsync();
                response.Data = packages.Select(p => new HouseMoverPackageDto
                {
                    PackageId = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl,    
                });
            }
            response.Message = "Packages retrieved successfully";
            response.Success = true;
            return response;
        }

        public async Task<Response<IEnumerable<HouseMovingTruckDto>>> GetTrucks(Guid? userId = null)
        {
            var response = new Response<IEnumerable<HouseMovingTruckDto>>();
            if (userId != null)
            {
                var houseMover = await _unitofWork.HouseMover.GetAsync(x => x.UserId == userId);
                if (houseMover == null)
                {
                    response.Message = "House cleaner not found";
                    return response;
                }
                var trucks = await _unitofWork.Context.HouseMovingTrucks
                    .Where(x => x.HouseMoverId == houseMover.Id)
                    .ToListAsync();
                response.Data = trucks.Select(x => new HouseMovingTruckDto
                {
                    TruckId = x.Id,
                    HouseMoverId = x.HouseMoverId,
                    Name = x.Name,
                    Price = x.Price,
                    ImageUrl = x.ImageUrl
                });
            }
            else
            {
                var packages = await _unitofWork.Context.HouseMovingTrucks.Where(x => x.HouseMoverId == null).ToListAsync();
                response.Data = packages.Select(x => new HouseMovingTruckDto
                {
                    TruckId = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    ImageUrl = x.ImageUrl
                });
            }
            response.Message = "Trucks retrieved successfully";
            response.Success = true;
            return response;
        }

        public async Task<Response<FreelancerDto>> RegisterFreeLancer(Guid userId, FreelancerRequest request, IFormFile file)
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

            var houseMover = new HouseMover
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
                //ServiceProviding = ServiceProviding.HouseMoving,
                ServiceProviderRole = ServiceProviderRole.Freelancer
            };
            var result = await _unitofWork.HouseMover.AddAsync(houseMover);

            var service = await _unitofWork.Context.ServiceProvidings
                .FirstOrDefaultAsync(x => x.Name == "House Moving");
            if (service == null)
            {
                service = new ServiceProviding
                {
                    Name = "House Moving",
                    Description = "House Moving Service",
                    //ImageUrl = "default_image_url" // Replace with actual default image URL
                };
                await _unitofWork.Context.ServiceProvidings.AddAsync(service);
                await _unitofWork.Context.SaveChangesAsync();
            }
            //user.ServiceProvidings.Add(service);
            user.ServiceProviderRole = houseMover.ServiceProviderRole;
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
                //ServiceProviding = result.ServiceProviding.ToString(),
                ServiceProviderRole = result.ServiceProviderRole.ToString()
            };
            return response;
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

            var houseMover = new HouseMover
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
                //ServiceProviding = ServiceProviding.HouseMoving,
                ServiceProviderRole = ServiceProviderRole.StoreOwner
            };
            await _unitofWork.HouseMover.AddAsync(houseMover);

            var service = await _unitofWork.Context.ServiceProvidings
                .FirstOrDefaultAsync(x => x.Name == "House Moving");
            if (service == null)
            {
                service = new ServiceProviding
                {
                    Name = "House Moving",
                    Description = "House Moving Service",
                    //ImageUrl = "default_image_url" // Replace with actual default image URL
                };
                await _unitofWork.Context.ServiceProvidings.AddAsync(service);
                await _unitofWork.Context.SaveChangesAsync();
            }
            //user.ServiceProvidings.Add(service);
            user.ServiceProviderRole = houseMover.ServiceProviderRole;
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

            response.Message = "Registration Succesfull";
            response.Success = true;
            response.Data = new ServiceProviderDto
            {
                UserId = user.Id,
                Id = houseMover.Id,
                CompanyName = houseMover.CompanyName,
                City = houseMover.City,
                RC = houseMover.RCorNIN,
                CompanyContact = houseMover.BusinessContact,
                Address = houseMover.AddressorLocation,
                Owner = houseMover.Owner,
                Locations = houseMover.Locations,
                YearsOfService = houseMover.YearsOfService,
                BusinessLogoUrl = houseMover.BusinessLogoorImageUrl,
                AccountName = houseMover.AccountName,
                AccountNumber = houseMover.AccountNumber,
                BankName = houseMover.BankName,
                ServiceProviderRole = houseMover.ServiceProviderRole.ToString(),
            };
            return response;
        }

        public async Task<Response<decimal>> SetTruckPricing(Guid truckId, decimal price)
        {
            var response = new Response<decimal>();
            var truck = await _unitofWork.Context.HouseMovingTrucks.FirstOrDefaultAsync(x  => x.Id == truckId);
            if (truck == null)
            {
                response.Message = "truck not found";
                response.Success = false;
                return response;
            }
            truck.Price = price;
            _unitofWork.Context.HouseMovingTrucks.Update(truck);
            await _unitofWork.Context.SaveChangesAsync();
            response.Message = "Truck Price Set";
            response.Success = true;
            response.Data = truck.Price;
            return response;

        }
    }
}
