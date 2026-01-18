using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.CarWash;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.CarWash;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.CarWash;
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

namespace CaptainTrackBackend.Application.Services.ServiceProviders.CarWash
{
    public class CarWasherService : ICarWasherService
    {
        private readonly IUnitofWork _unitofWork;
        public CarWasherService(IUnitofWork unitOfWork)
        {
            _unitofWork = unitOfWork;
        }

        public async Task<Response<CarWashItemDto>> AddCarWashItem(CarWashItemRequest request, Guid? userId = null)
        {
            var response = new Response<CarWashItemDto>();
            if(userId != null)
            {
                var carWasher = await _unitofWork.CarWasher.GetAsync(x => x.UserId == userId);
                if (carWasher == null)
                {
                    response.Success = false;
                    response.Message = "Car washer not found.";
                    return response;
                }
                var carWashItem = new CarWashItem
                {
                    CarWasherId = carWasher.Id,
                    Name = request.Name,
                    Price = request.Price
                };
                await _unitofWork.Context.CarWashItems.AddAsync(carWashItem);
                await _unitofWork.Context.SaveChangesAsync();
                response.Data = new CarWashItemDto
                {
                    Id = carWashItem.Id,
                    CarWasherId = carWashItem.CarWasherId,
                    Name = carWashItem.Name,
                    Price = carWashItem.Price
                };
            }
            else
            {
                var carWashItem = new CarWashItem
                {
                    Name = request.Name,
                    Price = request.Price
                };
                await _unitofWork.Context.CarWashItems.AddAsync(carWashItem);
                await _unitofWork.Context.SaveChangesAsync();
                response.Data = new CarWashItemDto
                {
                    Id = carWashItem.Id,
                    Name = carWashItem.Name,
                    Price = carWashItem.Price
                };

            }
            response.Success = true;
            response.Message = "Car wash item added successfully.";
            return response;
        }

        public async Task<Response<CarWashDto>> GetCarWash(Guid userId)
        {
            var response = new Response<CarWashDto>();
            var carWasher = await _unitofWork.CarWasher.Get(x => x.UserId == userId);
            if (carWasher == null)
            {
                response.Success = false;
                response.Message = "Car washer not found.";
                return response;
            }
            response.Data = new CarWashDto
            {
                Id = carWasher.Id,
                UserId = carWasher.UserId,
                CompanyName = carWasher.CompanyName,
                RC = carWasher.RCorNIN,
                Address = carWasher.AddressorLocation,
                City = carWasher.City,
                CompanyContact = carWasher.BusinessContact,
                Owner = carWasher.Owner,
                BusinessLogoUrl = carWasher.BusinessLogoorImageUrl,
                Locations = carWasher.Locations,
                YearsOfService = carWasher.YearsOfService,
                AccountNumber = carWasher.AccountNumber,
                BankName = carWasher.BankName,
                AccountName = carWasher.AccountName,
                CarWashItems = carWasher.CarWashItems.Select(item => new CarWashItemDto
                {
                    Id = item.Id,
                    CarWasherId = item.CarWasherId,
                    Name = item.Name,
                    Price = item.Price
                }).ToList()
            };
            response.Success = true;
            response.Message = "Car washer retrieved successfully.";
            return response;

        }

        public async Task<Response<IEnumerable<CarWashDto>>> GetCarwashes()
        {
            var response = new Response<IEnumerable<CarWashDto>>();
            var carWashers = await _unitofWork.CarWasher.GetAll();
            if (carWashers == null || !carWashers.Any())
            {
                response.Success = false;
                response.Message = "No car washers found.";
                return response;
            }
            response.Data = carWashers.Select(carWasher => new CarWashDto
            {
                Id = carWasher.Id,
                UserId = carWasher.UserId,
                CompanyName = carWasher.CompanyName,
                RC = carWasher.RCorNIN,
                Address = carWasher.AddressorLocation,
                City = carWasher.City,
                CompanyContact = carWasher.BusinessContact,
                Owner = carWasher.Owner,
                BusinessLogoUrl = carWasher.BusinessLogoorImageUrl,
                Locations = carWasher.Locations,
                YearsOfService = carWasher.YearsOfService,
                AccountNumber = carWasher.AccountNumber,
                BankName = carWasher.BankName,
                AccountName = carWasher.AccountName,
                //ServiceProviding = carWasher.ServiceProviding.ToString(),
                CarWashItems = carWasher.CarWashItems.Select(item => new CarWashItemDto
                {
                    Id = item.Id,
                    CarWasherId = item.CarWasherId,
                    Name = item.Name,
                    Price = item.Price
                }).ToList()
            });
            response.Success = true;
            response.Message = "Car washers retrieved successfully.";
            return response;
        }

        public async Task<Response<IEnumerable<CarWashItemDto>>> GetCarWashItems(Guid? userId = null)
        {
            var response = new Response<IEnumerable<CarWashItemDto>>();
            if(userId != null)
            {
                var carWasher = await _unitofWork.CarWasher.Get(CarWasher => CarWasher.UserId == userId);
                var items = await _unitofWork.Context.CarWashItems.Where(x => x.CarWasherId == carWasher.Id).ToListAsync();
                if (items == null || !items.Any())
                {
                    response.Success = false;
                    response.Message = "No car wash items found.";
                    return response;
                }
                response.Data = items.Select(item => new CarWashItemDto
                {
                    Id = item.Id,
                    CarWasherId = item.CarWasherId,
                    Name = item.Name,
                    Price = item.Price
                });
            }
            else
            {
                var items = await _unitofWork.Context.CarWashItems.Where(x => x.CarWasherId == null).ToListAsync();
                if (items == null || !items.Any())
                {
                    response.Success = false;
                    response.Message = "No car wash items found.";
                    return response;
                }
                response.Data = items.Select(item => new CarWashItemDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Price = item.Price
                });
            }
            response.Success = true;
            response.Message = "Car wash items retrieved successfully.";
            return response;
        }

        public async Task<Response<FreelancerDto>> RegisterFreelancer(Guid userId, FreelancerRequest request, IFormFile file)
        {
            var response = new Response<FreelancerDto>();
            var user = await _unitofWork.User.GetAsync(userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User Not found";
                return response;
            }
            if (file != null)
            {
                request.ImageUrl = await _unitofWork.FileUpload.UploadAsync(file);
            }
            var carWasher = new CarWasher
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
            await _unitofWork.CarWasher.AddAsync(carWasher);

            var service = await _unitofWork.Context.ServiceProvidings
                .FirstOrDefaultAsync(x => x.Name == "Car Washing");
            if (service == null)
            {
                service = new ServiceProviding
                {
                    Name = "Car Washing",
                    Description = "Car washing Service",
                    //ImageUrl = "default_image_url" // Replace with actual default image URL
                };
                await _unitofWork.Context.ServiceProvidings.AddAsync(service);
                await _unitofWork.Context.SaveChangesAsync();
            }
            //user.ServiceProvidings.Add(service);
            user.ServiceProviderRole = carWasher.ServiceProviderRole;
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
                    Id = carWasher.Id,
                    UserId = user.Id,
                    Name = user.FullName,
                    Email = user.Email,
                    BusinessContact = carWasher.BusinessContact,
                    NIN = carWasher.RCorNIN,
                    Location = carWasher.AddressorLocation,
                    City = carWasher.City,
                    BankName = carWasher.BankName,
                    AccountName = carWasher.AccountName,
                    AccountNumber = carWasher.AccountNumber,
                    ImageUrl = carWasher.BusinessLogoorImageUrl,
                    ServiceProviderRole = carWasher.ServiceProviderRole.ToString()
                }
            };
        }

        public async Task<Response<ServiceProviderDto>> RegisterStoreowner(Guid userId, ServiceProviderRequest request, IFormFileCollection files)
        {
            var response = new Response<ServiceProviderDto>();
            var user = await _unitofWork.User.GetAsync(userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
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

            var carWasher = new CarWasher
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
                //ServiceProviding = ServiceProviding.CarWash
            };
            await _unitofWork.CarWasher.AddAsync(carWasher);

            var service = await _unitofWork.Context.ServiceProvidings
                .FirstOrDefaultAsync(x => x.Name == "Car Wash");
            if (service == null)
            {
                service = new ServiceProviding
                {
                    Name = "Car Wash",
                    Description = "Car Wash Service",
                    //ImageUrl = "default_image_url" // Replace with actual default image URL
                };
                await _unitofWork.Context.ServiceProvidings.AddAsync(service);
                await _unitofWork.Context.SaveChangesAsync();
            }
            user.ServiceProviderRole = carWasher.ServiceProviderRole;
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

            response.Data = new ServiceProviderDto
            {
                Id = carWasher.Id,
                UserId = carWasher.UserId,
                CompanyName = carWasher.CompanyName,
                RC = carWasher.RCorNIN,
                Address = carWasher.AddressorLocation,
                City = carWasher.City,
                CompanyContact = carWasher.BusinessContact,
                Owner = carWasher.Owner,
                BusinessLogoUrl = carWasher.BusinessLogoorImageUrl,
                Locations = carWasher.Locations,
                YearsOfService = carWasher.YearsOfService,
                AccountNumber = carWasher.AccountNumber,
                BankName = carWasher.BankName,
                AccountName = carWasher.AccountName,
            };
            response.Success = true;
            response.Message = "Car washer registered successfully.";
            return response;

        }

        public async Task<Response<CarWashItemDto>> UpdateCarWashItem(Guid itemId, CarWashItemRequest request)
        {
            var response = new Response<CarWashItemDto>();
            var item = await _unitofWork.Context.CarWashItems.FirstOrDefaultAsync(x => x.Id == itemId);
            if (item == null)
            {
                response.Success = false;
                response.Message = "Car wash item not found.";
                return response;
            }
            item.Name = request.Name ?? item.Name;
            if (request.Price > 0)
            {
                item.Price = request.Price;
            }
            _unitofWork.Context.CarWashItems.Update(item);
            await _unitofWork.Context.SaveChangesAsync();
            response.Data = new CarWashItemDto
            {
                Id = item.Id,
                CarWasherId = item.CarWasherId,
                Name = item.Name,
                Price = item.Price
            };
            response.Success = true;
            response.Message = "Car wash item updated successfully.";
            return response;
        }
    }
}
