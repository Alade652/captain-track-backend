using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.VehicleTowing;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Domain.Enum;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.VehicleTowing;
using CaptainTrackBackend.Application.DTO.ServiceProviders.VehicleTowing;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.EntityFrameworkCore;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.House_Moving;
using CaptainTrackBackend.Domain.Identity;

namespace CaptainTrackBackend.Application.Services.ServiceProviders.VehicleTowing
{
    public class TruckOperatorService : ITruckOperatorService
    {
        private readonly IUnitofWork _unitofWork;
        public TruckOperatorService(IUnitofWork unitOfWork)
        {
            _unitofWork = unitOfWork;
        }

        public async Task<Response<TruckOperatorDto>> Get(Guid id)
        {
            var truckOperator = await _unitofWork.TruckOperator.Get( x => x.Id == id );
            if (truckOperator == null)
            {
                return new Response<TruckOperatorDto> { Message = "Does not exist", Success = false };
            }

            var truckOperatorDto = new TruckOperatorDto
            {
                CompanyName = truckOperator.CompanyName,
                CompanyContact = truckOperator.BusinessContact,
                Address = truckOperator.AddressorLocation,
                City = truckOperator.City,
                Locations = truckOperator.Locations,
                BusinessLogoUrl = truckOperator.BusinessLogoorImageUrl,
                RC = truckOperator.RCorNIN,
                AccountName = truckOperator.AccountName,
                AccountNumber = truckOperator.AccountNumber,
                BankName = truckOperator.BankName,
                YearsOfService = truckOperator.YearsOfService,
                Owner = truckOperator.Owner,
                ServiceProviderRole = truckOperator.ServiceProviderRole.ToString(),
                Trucks = truckOperator.trucks.Select(t => new TruckDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Amount = t.Amount,
                    ImageUrl = t.ImageUrl
                }).ToList()

            };
            return new Response<TruckOperatorDto>
            {
                Message = "TruckOperator gotten",
                Success = true,
                Data = truckOperatorDto
            };
        }

        public async Task<Response<TruckOperatorDto>> GetByUserId(Guid userId)

        {
            var truckOperator = await _unitofWork.TruckOperator.GetAsync(x => x.UserId == userId);
            if (truckOperator == null)
            {
                return new Response<TruckOperatorDto> { Message = "Does not exist", Success = false };
            }

            var truckOperatorDto = new TruckOperatorDto
            {
                CompanyName = truckOperator.CompanyName,
                CompanyContact = truckOperator.BusinessContact,
                Address = truckOperator.AddressorLocation,
                City = truckOperator.City,
                Locations = truckOperator.Locations,
                BusinessLogoUrl = truckOperator.BusinessLogoorImageUrl,
                RC = truckOperator.RCorNIN,
                AccountName = truckOperator.AccountName,
                AccountNumber = truckOperator.AccountNumber,
                BankName = truckOperator.BankName,
                YearsOfService = truckOperator.YearsOfService,
                Owner = truckOperator.Owner,
                ServiceProviderRole = truckOperator.ServiceProviderRole.ToString(),
                Trucks = truckOperator.trucks.Select(t => new TruckDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Amount = t.Amount,
                    ImageUrl = t.ImageUrl
                }).ToList()
            };
            return new Response<TruckOperatorDto>
            {
                Message = "TruckOperator gotten",
                Success = true,
                Data = truckOperatorDto
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

                var url = await _unitofWork.FileUpload.UploadAsync(file);

                switch (file.Name)
                {
                    case "BusinessLogoUrl":
                        request.BusinessLogoUrl = url;
                        break;
                }
            }

            var truckOperator = new TowTruckOperator
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
            await _unitofWork.TruckOperator.AddAsync(truckOperator);
            var service = await _unitofWork.Context.ServiceProvidings
                .FirstOrDefaultAsync(x => x.Name == "Towing");
            if (service == null)
            {
                service = new ServiceProviding
                {
                    Name = "Towing",
                    Description = "Towing Service",
                    //ImageUrl = "default_image_url" // Replace with actual default image URL
                };
                await _unitofWork.Context.ServiceProvidings.AddAsync(service);
                await _unitofWork.Context.SaveChangesAsync();
            }
            //user.ServiceProvidings.Add(service);
            user.ServiceProviderRole = truckOperator.ServiceProviderRole;
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
                    Id = truckOperator.Id,
                    CompanyName = truckOperator.CompanyName,
                    City = truckOperator.City,
                    RC = truckOperator.RCorNIN,
                    CompanyContact = truckOperator.BusinessContact,
                    Address = truckOperator.AddressorLocation,
                    Owner = truckOperator.Owner,
                    Locations = truckOperator.Locations,
                    YearsOfService = truckOperator.YearsOfService,
                    BusinessLogoUrl = truckOperator.BusinessLogoorImageUrl,
                    AccountName = request.AccountName,
                    AccountNumber = request.AccountNumber,
                    BankName = request.BankName,
                    ServiceProviderRole = truckOperator.ServiceProviderRole.ToString(),
                }
            };
        }

        public async Task<Response<List<TruckOperatorDto>>> GetAll()
        {
            var truckOperators = await _unitofWork.TruckOperator.GetAll();
            if (truckOperators == null)
            {
                return new Response<List<TruckOperatorDto>>
                {
                    Message = "No dry cleaners found",
                    Success = false,
                };
            }
            var truckOperatorDtos = truckOperators.Select(d => new TruckOperatorDto
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
                ServiceProviderRole = d.ServiceProviderRole.ToString(),
                Trucks = d.trucks.Select(t => new TruckDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Amount = t.Amount,
                    ImageUrl = t.ImageUrl
                }).ToList()
            }).ToList();
            return new Response<List<TruckOperatorDto>>
            {
                Message = "Dry cleaners retrieved successfully",
                Success = true,
                Data = truckOperatorDtos
            };
            //var truckOperators = await _dryCleanerRepository.GetAllAsync(x => x.City == location);
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
            var truckOperator = new TowTruckOperator
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
            await _unitofWork.TruckOperator.AddAsync(truckOperator);

            var service = await _unitofWork.Context.ServiceProvidings
                .FirstOrDefaultAsync(x => x.Name == "Towing");
            if (service == null)
            {
                service = new ServiceProviding
                {
                    Name = "Towing",
                    Description = "Towing Service",
                    //ImageUrl = "default_image_url" // Replace with actual default image URL
                };
                await _unitofWork.Context.ServiceProvidings.AddAsync(service);
                await _unitofWork.Context.SaveChangesAsync();
            }
            //user.ServiceProvidings.Add(service);
            user.ServiceProviderRole = truckOperator.ServiceProviderRole;
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
                    Id = truckOperator.Id,
                    UserId = user.Id,
                    Name = user.FullName,
                    Email = user.Email,
                    BusinessContact = truckOperator.BusinessContact,
                    NIN = truckOperator.RCorNIN,
                    Location = truckOperator.AddressorLocation,
                    City = truckOperator.City,
                    BankName = truckOperator.BankName,
                    AccountName = truckOperator.AccountName,
                    AccountNumber = truckOperator.AccountNumber,
                    ImageUrl = truckOperator.BusinessLogoorImageUrl,
                    ServiceProviderRole = truckOperator.ServiceProviderRole.ToString()
                }
            };
        }

        public async Task<Response<TruckDto>> AddTruck(TruckRequest request, Guid? truckOperatorUserId = null)
        {
            var response = new Response<TruckDto>();
            TowTruckOperator truckOperator = null;
            if(truckOperatorUserId != null)
            {
                truckOperator = await _unitofWork.TruckOperator.Get(x => x.UserId == truckOperatorUserId);
                if (truckOperator == null)
                {
                    response.Message = "Truck operator not found";
                    response.Success = false;
                    return response;
                }
            }
            var truck = new Truck();
            if(truckOperator != null)
            {
                truck.TowTruckOperatorId = truckOperator.Id;
                truck.Name = request.Name;
                truck.Amount = request.Amount;
                truck.ImageUrl = request.Image != null ? await _unitofWork.FileUpload.UploadAsync(request.Image) : null;
            }
            else
            {
                truck.Name = request.Name;
                truck.Amount = request.Amount;
                truck.ImageUrl = request.Image != null ? await _unitofWork.FileUpload.UploadAsync(request.Image) : null;
            }
            await _unitofWork.Context.Trucks.AddAsync(truck);
            await _unitofWork.Context.SaveChangesAsync();
            response.Message = "Truck added successfully";
            response.Success = true;
            response.Data = new TruckDto
            {
                Id = truck.Id,
                TowTruckOperatorId = truck.TowTruckOperatorId,
                Name = truck.Name,
                Amount = truck.Amount,
                ImageUrl = truck.ImageUrl
            };
            return response;
        }

        public async Task<Response<List<TruckDto>>> GetTrucks(Guid? truckOperatorUserId = null)
        {
            var response = new Response<List<TruckDto>>();
            if (truckOperatorUserId != null)
            {
                var truckOperator = await _unitofWork.TruckOperator.Get(x => x.UserId == truckOperatorUserId);
                if (truckOperator != null)
                {
                    response.Data = truckOperator.trucks.Select(t => new TruckDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Amount = t.Amount,
                        ImageUrl = t.ImageUrl
                    }).ToList();
                    response.Message = "Trucks retrieved successfully";
                    response.Success = true;
                    return response;
                }
                else
                {
                    response.Message = "Truck operator not found";
                    response.Success = false;
                    return response;
                }
            }
            var trucks = await _unitofWork.Context.Trucks.Where(x => x.TowTruckOperatorId == null).ToListAsync();
            if (trucks == null || !trucks.Any())
            {
                response.Message = "No trucks found";
                response.Success = false;
                return response;
            }
            response.Data = trucks.Select(t => new TruckDto
            {
                Id = t.Id,
                Name = t.Name,
                Amount = t.Amount,
                ImageUrl = t.ImageUrl
            }).ToList();
            response.Message = "Trucks retrieved successfully";
            response.Success = true;
            return response;
        }
    }
}
