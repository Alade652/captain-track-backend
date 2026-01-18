using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.Courier;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.Courier;
using CaptainTrackBackend.Application.DTO.ServiceProviders.VehicleTowing;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.Courier;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.VehicleTowing;
using CaptainTrackBackend.Domain.Enum;
using CaptainTrackBackend.Domain.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Services.ServiceProviders.Courier
{
    public class RiderorParkService : IRiderorParkService
    {
        private readonly IUnitofWork _unitofWork;
        public RiderorParkService(IUnitofWork unitOfWork)
        {
            _unitofWork = unitOfWork;
        }

        public async Task<Response<CourierVehicleDto>> AddItem(CourierVehicleRequest request, Guid? userId = null)
        {
            var response = new Response<CourierVehicleDto>();
            RiderorPark riderorPark = null;
            if (userId != null)
            {
                riderorPark = await _unitofWork.RiderorPark.GetAsync(x => x.UserId == userId);
                if (riderorPark == null)
                {
                    response.Message = "Truck operator not found";
                    response.Success = false;
                    return response;
                }
            }
            var courierVehicle = new CourierVehicle();
            if (riderorPark != null)
            {
                courierVehicle.ParkId = riderorPark.Id;
                courierVehicle.VehicleType = request.VehicleType;
                courierVehicle.Price = request.Price;
                courierVehicle.ImageUrl = request.Image != null ? await _unitofWork.FileUpload.UploadAsync(request.Image) : null;
            }
            else
            {
                courierVehicle.VehicleType = request.VehicleType;
                courierVehicle.Price = request.Price;
                courierVehicle.ImageUrl = request.Image != null ? await _unitofWork.FileUpload.UploadAsync(request.Image) : null;
            }
            await _unitofWork.Context.CourierVehicles.AddAsync(courierVehicle);
            await _unitofWork.Context.SaveChangesAsync();
            response.Message = "Truck added successfully";
            response.Success = true;
            response.Data = new CourierVehicleDto
            {
                VehicleId = courierVehicle.Id,
                ParkId = courierVehicle.ParkId,
                VehicleType = courierVehicle.VehicleType,
                Price = courierVehicle.Price,
                ImageUrl = courierVehicle.ImageUrl
            };
            return response;
        }

        public async Task<Response<IEnumerable<CourierVehicleDto>>> GetItems(Guid? userId = null)
        {
            var response = new Response<IEnumerable<CourierVehicleDto>>();
            if (userId != null)
            {
                var park = await _unitofWork.RiderorPark.GetRiderorParkAsync(x => x.UserId == userId);
                if (park != null)
                {
                    response.Data = park.CourierVehicles.Select(t => new CourierVehicleDto
                    {
                        ParkId = t.Id,
                        VehicleType = t.VehicleType,
                        Price = t.Price,
                        ImageUrl = t.ImageUrl
                    }).ToList();
                    response.Message = "Vehicles retrieved successfully";
                    response.Success = true;
                    return response;
                }
                else
                {
                    response.Message = "Vehicles operator not found";
                    response.Success = false;
                    return response;
                }
            }

            var vehicles = await _unitofWork.Context.CourierVehicles.Where(x => x.ParkId == null).ToListAsync();
            if (vehicles == null || !vehicles.Any())
            {
                response.Message = "No vehicles found";
                response.Success = false;
                return response;
            }
            response.Data = vehicles.Select(t => new CourierVehicleDto
            {
                ParkId = t.Id,
                VehicleType = t.VehicleType,
                Price = t.Price,
                ImageUrl = t.ImageUrl
            }).ToList();
            response.Message = "Trucks retrieved successfully";
            response.Success = true;
            return response;
        }

        public async Task<Response<ParkDto>> GetPark(Guid userId)
        {
            var response = new Response<ParkDto>();
            var park = await _unitofWork.RiderorPark.GetRiderorParkAsync(x => x.UserId == userId && x.ServiceProviderRole == ServiceProviderRole.StoreOwner);
            if (park == null)
            {
                response.Success = false;
                response.Message = "Not found";
                return response;
            }
            response.Data = new ParkDto
            {
                Id = park.Id,
                UserId = userId,
                RC = park.RCorNIN,
                Address = park.AddressorLocation,
                City = park.City,
                CompanyContact = park.BusinessContact,
                CompanyName = park.CompanyName,
                BusinessLogoUrl = park.BusinessLogoorImageUrl,
                Locations = park.Locations,
                YearsOfService = park.YearsOfService,
                AccountName = park.AccountName,
                AccountNumber = park.AccountNumber,
                BankName = park.BankName,
                ServiceProviderRole = park.ServiceProviderRole.ToString(),
                Vehicles = park.CourierVehicles.Select(x => new CourierVehicleDto
                {
                    VehicleId = x.Id,
                    VehicleType = x.VehicleType,
                    Price = x.Price,
                    ImageUrl = x.ImageUrl
                }).ToList()

            };

            response.Success = true;
            response.Message = "Park gotten";
            return response;
        }

        public async Task<Response<IEnumerable<ParkDto>>> GetParks()
        {
            var response = new Response<IEnumerable<ParkDto>>();
            var parks = await _unitofWork.RiderorPark.GetAllRiderorParksAsync();
            if (parks == null)
            {
                response.Success = false;
                response.Message = "No parks";
                return response;
            }

            response.Data = parks.Select(park => new ParkDto
            {
                Id = park.Id,
                UserId = park.UserId,
                RC = park.RCorNIN,
                Address = park.AddressorLocation,
                City = park.City,
                CompanyContact = park.BusinessContact,
                CompanyName = park.CompanyName,
                BusinessLogoUrl = park.BusinessLogoorImageUrl,
                Locations = park.Locations,
                YearsOfService = park.YearsOfService,
                AccountName = park.AccountName,
                AccountNumber = park.AccountNumber,
                BankName = park.BankName,
                ServiceProviderRole = park.ServiceProviderRole.ToString(),
                Vehicles = park.CourierVehicles.Select(x => new CourierVehicleDto
                {
                    VehicleId = x.Id,
                    VehicleType = x.VehicleType,
                    Price = x.Price,
                    ImageUrl = x.ImageUrl
                }).ToList()

            });

            response.Success = true;
            response.Message = "Parks gotten";
            return response;
        }

        public async Task<Response<FreelancerDto>> GetRider(Guid userId)
        {
            var response = new Response<FreelancerDto>();
            var rider = await _unitofWork.RiderorPark.GetRiderorParkAsync(x => x.UserId == userId && 
            x.ServiceProviderRole == ServiceProviderRole.Freelancer);
            if (rider == null)
            {
                response.Success = false;
                response.Message = "Not found";
                return response;
            }
            response.Data = new FreelancerDto
            {
                Id = rider.Id,
                UserId = rider.UserId,
                Name = rider.User.FullName,
                Email = rider.User.Email,
                ImageUrl = rider.BusinessLogoorImageUrl,
                BusinessContact = rider.BusinessContact,
                NIN = rider.RCorNIN,
                Location = rider.AddressorLocation,
                City = rider.City,
                AccountName = rider.AccountName,
                AccountNumber = rider.AccountNumber,
                BankName = rider.AccountName,
                ServiceProviderRole = rider.ServiceProviderRole.ToString()
            };

            response.Success = true;
            response.Message = "Rider gotten";
            return response;
        }

        public async Task<Response<ServiceProviderDto>> RegisterPark(Guid userId, ServiceProviderRequest request, IFormFileCollection files)
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

                var url = await _unitofWork.FileUpload.UploadAsync(file);

                switch (file.Name)
                {
                    case "BusinessLogoUrl":
                        request.BusinessLogoUrl = url;
                        break;
                }
            }

            var park = new RiderorPark
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
            await _unitofWork.RiderorPark.AddAsync(park);
            var service = await _unitofWork.Context.ServiceProvidings
                .FirstOrDefaultAsync(x => x.Name == "Courier");
            if (service == null)
            {
                service = new ServiceProviding
                {
                    Name = "Courier",
                    Description = "Courier Service",
                };
                await _unitofWork.Context.ServiceProvidings.AddAsync(service);
                await _unitofWork.Context.SaveChangesAsync();
            }
            //user.ServiceProvidings.Add(service);
            user.ServiceProviderRole = park.ServiceProviderRole;
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
                    Id = park.Id,
                    CompanyName = park.CompanyName,
                    City = park.City,
                    RC = park.RCorNIN,
                    CompanyContact = park.BusinessContact,
                    Address = park.AddressorLocation,
                    Owner = park.Owner,
                    Locations = park.Locations,
                    YearsOfService = park.YearsOfService,
                    BusinessLogoUrl = park.BusinessLogoorImageUrl,
                    AccountName = request.AccountName,
                    AccountNumber = request.AccountNumber,
                    BankName = request.BankName,
                    ServiceProviderRole = park.ServiceProviderRole.ToString(),
                }
            };
        }

        public async Task<Response<FreelancerDto>> RegisterRider(Guid userId, FreelancerRequest request, IFormFile file)
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
            var rider = new RiderorPark
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
            await _unitofWork.RiderorPark.AddAsync(rider);


            var service = await _unitofWork.Context.ServiceProvidings
                .FirstOrDefaultAsync(x => x.Name == "Courier");
            if (service == null)
            {
                service = new ServiceProviding
                {
                    Name = "Courier",
                    Description = "Courier Service",
                };
                await _unitofWork.Context.ServiceProvidings.AddAsync(service);
                await _unitofWork.Context.SaveChangesAsync();
            }

            user.ServiceProviderRole = rider.ServiceProviderRole;
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
                    Id = rider.Id,
                    UserId = user.Id,
                    Name = user.FullName,
                    Email = user.Email,
                    BusinessContact = rider.BusinessContact,
                    NIN = rider.RCorNIN,
                    Location = rider.AddressorLocation,
                    City = rider.City,
                    BankName = rider.BankName,
                    AccountName = rider.AccountName,
                    AccountNumber = rider.AccountNumber,
                    ImageUrl = rider.BusinessLogoorImageUrl,
                    ServiceProviderRole = rider.ServiceProviderRole.ToString()
                }
            };
        }
    }
}
