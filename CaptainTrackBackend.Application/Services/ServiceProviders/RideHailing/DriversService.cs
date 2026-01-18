using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.RideHailing;
using CaptainTrackBackend.Application.Abstraction.Interface.Services;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.RideHailing;
using CaptainTrackBackend.Application.Authentcication;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders.Ridehailing;
using CaptainTrackBackend.Application.Services.FileUpload;
using CaptainTrackBackend.Domain.Entities.ServiceProviders;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.House_Moving;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.RideHailing;
using CaptainTrackBackend.Domain.Enum;
using CaptainTrackBackend.Domain.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CaptainTrackBackend.Application.Services.ServiceProviders.RideHailing
{
    public class DriversService : IDriversServices
    {
        private readonly IUnitofWork _unitofWork;
        private readonly IOTPService _oTPService;

        public DriversService(IUnitofWork unitofWork, IOTPService oTPService)
        {
            // Constructor logic here
            _unitofWork = unitofWork;
            _oTPService = oTPService;
        }

        public Task<Response<bool>> Approve(Guid id)
        {
            var getDriver = _unitofWork.Driver.GetAsync(x => x.Id == id);
            if (getDriver.Result == null)
            {
                return Task.FromResult(new Response<bool>
                {
                    Message = "Driver not found",
                    Success = false,
                    Data = false
                });
            }
            getDriver.Result.IsApproved = true;
            _unitofWork.Driver.UpdateAsync(getDriver.Result);
            return Task.FromResult(new Response<bool>
            {
                Message = "Driver approved successfully",
                Success = true,
                Data = true
            });
        }

        public Task<Response<bool>> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Response<IList<DriverDto>>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<Response<DriverDto>> Get(Guid id)
        {
            var getDriver = await _unitofWork.Driver.GetAsync(id);
            if (getDriver == null)
            {
                return new Response<DriverDto>
                {
                    Message = "Driver not found",
                    Success = false,
                    Data = null
                };
            }
            var driverDto = new DriverDto
            {
                FullName = getDriver.FullName,
                Email = getDriver.User.Email,
                PhoneNumber = getDriver.PhoneNumber,
                ImageUrl = getDriver.ImageUrl,
                LicenseUrl = getDriver.LicenseUrl,
                PlateNumber = getDriver.PlateNumber,
                VehicleColor = getDriver.VehicleColor,
                VehicleModel = getDriver.VehicleModel,
                VehicleImageUrl = getDriver.VehicleImageUrl,
                VehicleRegistrationUrl = getDriver.VehicleRegistrationUrl,
                RoadWorthyCertificateUrl = getDriver.RoadWorthyCertificateUrl,
                YearOfManufacture = getDriver.YearOfManufacture,
                AccountNumber = getDriver.AccountNumber,
                BankName = getDriver.BankName,
                AccountName = getDriver.AccountName,
                NIN = getDriver.NIN,
                LicenceExpiryDate = getDriver.LicenceExpiryDate,
                IsApproved = getDriver.IsApproved,
                IsAvailable = getDriver.IsAvailable,
                CurrentLocation = getDriver.CurrentLocation,
            };
            return new Response<DriverDto>
            {
                Message = "Driver found successfully",
                Success = true,
                Data = driverDto
            };
        }

        public async Task<Response<DriverDto>> Get(string email)
        {
            var getDriver = await _unitofWork.Driver.GetAsync(x => x.User.Email == email);
            if (getDriver == null)
            {
                return new Response<DriverDto>
                {
                    Message = "Driver not found",
                    Success = false,
                    Data = null
                };
            }
            var driverDto = new DriverDto
            {
                FullName = getDriver.FullName,
                Email = getDriver.User.Email,
                PhoneNumber = getDriver.PhoneNumber,
                ImageUrl = getDriver.ImageUrl,
                LicenseUrl = getDriver.LicenseUrl,
                PlateNumber = getDriver.PlateNumber,
                VehicleColor = getDriver.VehicleColor,
                VehicleModel = getDriver.VehicleModel,
                VehicleImageUrl = getDriver.VehicleImageUrl,
                VehicleRegistrationUrl = getDriver.VehicleRegistrationUrl,
                RoadWorthyCertificateUrl = getDriver.RoadWorthyCertificateUrl,
                YearOfManufacture = getDriver.YearOfManufacture,
                AccountNumber = getDriver.AccountNumber,
                BankName = getDriver.BankName,
                AccountName = getDriver.AccountName,
                NIN = getDriver.NIN,
                LicenceExpiryDate = getDriver.LicenceExpiryDate,
                IsApproved = getDriver.IsApproved,
                IsAvailable = getDriver.IsAvailable,
                CurrentLocation = getDriver.CurrentLocation,
            };
            return new Response<DriverDto>
            {
                Message = "Driver found successfully",
                Success = true,
                Data = driverDto
            };
        }

        public async Task<Response<DriverDto>> GetByUserId(Guid userId)
        {
            var getDriver = await _unitofWork.Driver.GetAsync(x => x.UserId == userId);
            var getUser = await _unitofWork.User.GetAsync(x => x.Id == userId);
            if (getDriver == null)
            {
                return new Response<DriverDto>
                {
                    Message = "Driver not found",
                    Success = false,
                    Data = null
                };
            }
            var driverDto = new DriverDto
            {
                FullName = getDriver.FullName,
                Email = getUser.Email,
                PhoneNumber = getDriver.PhoneNumber,
                ImageUrl = getDriver.ImageUrl,
                LicenseUrl = getDriver.LicenseUrl,
                PlateNumber = getDriver.PlateNumber,
                VehicleColor = getDriver.VehicleColor,
                VehicleModel = getDriver.VehicleModel,
                VehicleImageUrl = getDriver.VehicleImageUrl,
                VehicleRegistrationUrl = getDriver.VehicleRegistrationUrl,
                RoadWorthyCertificateUrl = getDriver.RoadWorthyCertificateUrl,
                YearOfManufacture = getDriver.YearOfManufacture,
                AccountNumber = getDriver.AccountNumber,
                BankName = getDriver.BankName,
                AccountName = getDriver.AccountName,
                NIN = getDriver.NIN,
                LicenceExpiryDate = getDriver.LicenceExpiryDate,
                IsApproved = getDriver.IsApproved,
                IsAvailable = getDriver.IsAvailable,
                CurrentLocation = getDriver.CurrentLocation,
            };
            return new Response<DriverDto>
            {
                Message = "Driver found successfully",
                Success = true,
                Data = driverDto
            };
        }

        public async Task<Response<DriverDto>> InitReg(DriverInitRequest initRequest)
        {
            var user = await _unitofWork.User.GetAsync(x => x.Email == initRequest.Email);
            if (user != null)
            {
                return new Response<DriverDto>
                {
                    Message = "You're already registered, do you wish to provide another service",
                    Success = false,
                };
            }

            var newUser = new User
            {
                FullName = initRequest.FullName,
                Email = initRequest.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(initRequest.Password),
                PhoneNumber = initRequest.PhoneNumber,
                Role = Role.ServiceProvider
                
            };
            await _unitofWork.User.AddAsync(newUser);
            var driver = new Driver
            {
                UserId = newUser.Id,
                FullName = initRequest.FullName,
                PhoneNumber = initRequest.PhoneNumber,
            };

            await _oTPService.GenerateOTP(newUser.Id);
            await _unitofWork.Driver.AddAsync(driver);

            return new Response<DriverDto>
            {
                Message = "Initial Registration Succesful",
                Success = true,
                Data = new DriverDto
                {
                    UserId = driver.UserId,
                    Id = driver.Id,
                    FullName = driver.FullName,
                    Email = driver.User.Email,
                    PhoneNumber = driver.PhoneNumber
                }
            };
        }

        public async Task<Response<DriverDto>> Register(Guid userId, DriverRequestDto driverDto, IFormFileCollection files)
        {
            var user = await _unitofWork.User.GetAsync(userId);
            if (user == null)
            {
                return new Response<DriverDto>
                {
                    Message = "User not found ",
                    Success = false,
                    Data = null
                };
            }

            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                var url = await _unitofWork.FileUpload.UploadAsync(file);

                switch (file.Name)
                {
                    case "ImageUrl":
                        driverDto.ImageUrl = url;
                        break;
                    case "LicenceUrl":
                        driverDto.LicenseUrl = url;
                        break;
                    case "VehicleimageUrl":
                        driverDto.VehicleImageUrl = url;
                        break;
                    case "RoadworthycertificateUrl":
                        driverDto.RoadWorthyCertificateUrl = url;
                        break;
                    case "VehicleRegistrationUrl":
                        driverDto.VehicleRegistrationUrl = url;
                        break;
                }
            }


            var driver = new Driver
            {
                UserId = userId,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                NIN = driverDto.NIN,
                LicenceExpiryDate = driverDto.LicenceExpiryDate,
                YearOfManufacture = driverDto.YearOfManufacture,
                ImageUrl = driverDto.ImageUrl,
                LicenseUrl = driverDto.LicenseUrl,
                VehicleImageUrl = driverDto.VehicleImageUrl,
                VehicleRegistrationUrl = driverDto.VehicleRegistrationUrl,
                RoadWorthyCertificateUrl = driverDto.RoadWorthyCertificateUrl,
                AccountNumber = driverDto.AccountNumber,
                BankName = driverDto.BankName,
                AccountName = driverDto.AccountName,
                PlateNumber = driverDto.PlateNumber,
                VehicleColor = driverDto.VehicleColor,
                VehicleModel = driverDto.VehicleModel,
                IsApproved = false,
            };
            await _unitofWork.Driver.AddAsync(driver);
            var service = await _unitofWork.Context.ServiceProvidings
                     .FirstOrDefaultAsync(x => x.Name == "Ride Hailing");
            if (service == null)
            {
                service = new ServiceProviding
                {
                    Name = "Ride Hailing",
                    Description = "Ride Hailing",
                    //ImageUrl = "default_image_url" // Replace with actual default image URL
                };
                await _unitofWork.Context.ServiceProvidings.AddAsync(service);
                await _unitofWork.Context.SaveChangesAsync();
            }
            //user.ServiceProvidings.Add(service);
            await _unitofWork.User.UpdateAsync(user);

            var userServiceproviding = new UserServiceProviding
            {
                UserId = user.Id,
                ServiceProvidingId = service.Id,
            };
            await _unitofWork.Context.UserServiceProvidings.AddAsync(userServiceproviding);
            await _unitofWork.Context.SaveChangesAsync();
            var newDriver = new DriverDto
            {
                Id = driver.Id,
                UserId = driver.UserId,
                NIN = driver.NIN,
                FullName = driver.FullName,
                PhoneNumber = driver.PhoneNumber,
                Email = driver.User.Email,
                ImageUrl = driverDto.ImageUrl,
                LicenseUrl = driverDto.LicenseUrl,
                PlateNumber = driverDto.PlateNumber,
                VehicleImageUrl = driverDto.VehicleImageUrl,
                VehicleColor = driverDto.VehicleColor,
                VehicleModel = driverDto.VehicleModel,
                LicenceExpiryDate = driverDto.LicenceExpiryDate,
                YearOfManufacture = driverDto.YearOfManufacture,
                AccountNumber = driverDto.AccountNumber,
                BankName = driverDto.BankName,
                AccountName = driverDto.AccountName,
                VehicleRegistrationUrl = driverDto.VehicleRegistrationUrl,
                RoadWorthyCertificateUrl = driverDto.RoadWorthyCertificateUrl,
            };

            var emailDto = new EmailDto
            {
                To = driver.User.Email,
                Subject = "Welcome to CaptainTrack",
                Body = $"Hello {newDriver.FullName},<br/>You've registered as a driver on CaptainTrack. Pls await verification"
            };
            await _unitofWork.Email.SendEmailAsync(emailDto);
            return new Response<DriverDto>
            {
                Message = "Driver registered successfully",
                Success = true,
                Data = newDriver
            };
        }

        public async Task<Response<DriverDto>> Update(Guid id, DriverUpdateDto driverDto)
        {
            var driver = await _unitofWork.Driver.GetAsync(id);
            if (driver == null)
            {
                return new Response<DriverDto>
                {
                    Message = "Driver not found",
                    Success = false,
                    Data = null
                };
            }
            driver.FullName = driverDto.FullName ?? driver.FullName;
            driver.User.Email = driverDto.Email ?? driver.User.Email;
            driver.PhoneNumber = driverDto.PhoneNumber ?? driver.PhoneNumber;
/*            driver.Image = driverDto.Image ?? driver.Image;
            driver.License = driverDto.License ?? driver.License;
            driver.VehicleRegistration = driverDto.VehicleRegistration ?? driver.VehicleRegistration;
            driver.RoadWorthyCertificate = driverDto.RoadWorthyCertificate ?? driver.RoadWorthyCertificate;
            driver.VehicleImage = driverDto.VehicleImage ?? driver.VehicleImage;*/
            driver.VehicleColor = driverDto.VehicleColor ?? driver.VehicleColor;
            driver.VehicleModel = driverDto.VehicleModel ?? driver.VehicleModel;
            driver.PlateNumber = driverDto.PlateNumber ?? driver.PlateNumber;
            driver.YearOfManufacture = driverDto.YearOfManufacture ?? driver.YearOfManufacture;
            driver.AccountNumber = driverDto.AccountNumber ?? driver.AccountNumber;
            driver.BankName = driverDto.BankName ?? driver.BankName;
            driver.AccountName = driverDto.AccountName ?? driver.AccountName;
            driver.NIN = driverDto.NIN ?? driver.NIN;
            driver.LicenceExpiryDate = driverDto.LicenceExpiryDate ?? driver.LicenceExpiryDate;


            await _unitofWork.User.UpdateAsync(driver.User);

            var updateDriver =  await _unitofWork.Driver.UpdateAsync(driver);
            return new Response<DriverDto>
            {
                Message = "Driver updated successfully",
                Success = true,
                Data = new DriverDto
                {
                    FullName = updateDriver.FullName,
                    Email = updateDriver.FullName,
                    PhoneNumber = updateDriver.FullName,
/*                    Image = updateDriver.Image,
                    License = updateDriver.License,
                    VehicleImage = updateDriver.VehicleImage,
                    VehicleRegistration = updateDriver.VehicleRegistration,
                    RoadWorthyCertificate = updateDriver.RoadWorthyCertificate,*/
                    PlateNumber = updateDriver.PlateNumber,
                    VehicleColor = updateDriver.VehicleColor,
                    VehicleModel = updateDriver.VehicleModel,
                    YearOfManufacture = updateDriver.YearOfManufacture,
                    AccountNumber = updateDriver.AccountNumber,
                    BankName = updateDriver.BankName,
                    AccountName = updateDriver.AccountName,
                    NIN = updateDriver.NIN,
                    LicenceExpiryDate = updateDriver.LicenceExpiryDate,
                }
            };
        }

        public async Task<Response<IList<DriverDto>>> GetAvailable()
        {
            var availableDrivers = await _unitofWork.Driver.GetAllByExpression(x => x.IsAvailable == true);
            if(availableDrivers == null)
            {
                return new Response<IList<DriverDto>>
                {
                    Message = "No available drivers found",
                    Success = false,
                    Data = null
                };
            }
            var driverDtos = availableDrivers.Select(driver => new DriverDto
            {
                FullName = driver.FullName,
                PhoneNumber = driver.PhoneNumber,
                ImageUrl = driver.ImageUrl,
                PlateNumber = driver.PlateNumber,
                VehicleColor = driver.VehicleColor,
                VehicleModel = driver.VehicleModel,
                CurrentLocation = driver.CurrentLocation,
            }).ToList();
            return new Response<IList<DriverDto>>
            {
                Message = "Available drivers found successfully",
                Success = true,
                Data = driverDtos
            };
        }

    }
}
