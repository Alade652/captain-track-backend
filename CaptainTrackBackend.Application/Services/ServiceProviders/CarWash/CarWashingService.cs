using CaptainTrackBackend.Application.Abstraction.Interface.Maps;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.CarWash;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.MapModels;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.Ambulance;
using CaptainTrackBackend.Application.DTO.ServiceProviders.CarWash;
using CaptainTrackBackend.Application.Services.ServiceProviders.Ambulance;
using CaptainTrackBackend.Domain.Entities;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.CarWash;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.HouseCleaning;
using CaptainTrackBackend.Domain.Enum;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Services.ServiceProviders.CarWash
{
    public class CarWashingService : ICarWashingService
    {
        private readonly IUnitofWork _unitofWork;
        private readonly IMapServices _mapServices;
        private readonly IHubContext<NegotiationHub> _hubContext;
        private readonly ILogger<CarWashingService> _logger;

        public CarWashingService(IUnitofWork unitOfWork, IMapServices mapServices, IHubContext<NegotiationHub> hubContext, ILogger<CarWashingService> logger)
        {
            _unitofWork = unitOfWork;
            _mapServices = mapServices;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task<NotificationResult> NotifyServiceProviders(CarWashingDto booking)
        {
            var response = new NotificationResult();
            try
            {
                var providers = await _unitofWork.CarWasher.GetAllByExpression(x => x.ServiceProviderRole == ServiceProviderRole.Freelancer /*x.IsAvailable == true*/);
                if (providers == null || !providers.Any())
                {
                    response.Success = false;
                    response.Message = $"No service providers availabe";
                    //_logger.LogWarning(response.Message);
                    return response;
                }
                foreach (var provider in providers)
                {
                    try
                    {
                        /*var distdura = await _mapServices.GetDistanceAndDurationAsync(provider.AddressorLocation, booking.Location);
                        var result = await _mapServices.GetDistanceAsync(provider.AddressorLocation, booking.Location);
                        var element = result.rows.FirstOrDefault()?.elements.FirstOrDefault();
                        if (element == null || element.status != "OK")
                        {
                            _logger.LogWarning($"Failed to calculate distance");
                            continue;
                        }

                        if (element != null && element.status == "OK" && distdura.DistanceMeters <= 5000)
                        {

                            booking.DistancetoLocation = element.distance.text;
                            booking.DurationtoLocation = element.duration.text;


                            *//* await _hubContext.Clients.User(provider.Id.ToString())
                                 .SendAsync("ReceivePendingBooking", booking);*//*
                            await _hubContext.Clients.Group(provider.Id.ToString())
                                .SendAsync("ReceivePendingBooking", booking);

                            response.ProvidersNotified++;
                            _logger.LogInformation($"Notified provider {provider.Id}");
                        }*/

                        await _hubContext.Clients.Group(provider.Id.ToString())
                            .SendAsync("ReceivePendingBooking", booking);
                        response.ProvidersNotified++;

                        _logger.LogInformation($"Notified provider {provider.Id}");

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error notifying provider {provider.Id}");
                    }

                }
                if (response.ProvidersNotified == 0)
                {
                    response.Success = false;
                    response.Message = $"No service providers near .";
                    _logger.LogWarning(response.Message);
                }
                else
                {
                    response.Message = $"Successfully notified {response.ProvidersNotified} providers.";
                    _logger.LogInformation(response.Message);
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to notify providers for : {ex.Message}";
                _logger.LogError(ex, "Error in NotifyServiceProvidersAsync for booking ID {BookingId}", booking.CarWashingId);
            }

            return response;
        }

        public async Task<Response<bool>> AcceptBooking(Guid carWashingId)
        {
            var response = new Response<bool>();
            var carWashing = await _unitofWork.CarWashing.GetAsync(x => x.Id == carWashingId);
            if (carWashing == null)
            {
                response.Message = "CarWashing Not found";
                response.Success = false;
                return response;
            }

            carWashing.ServiceStatus = ServiceStatus.InProgress;
            await _unitofWork.CarWashing.UpdateAsync(carWashing);
            response.Message = "Booking accepted successfully";
            response.Success = true;
            response.Data = true;
            return response;
        }

        public async Task<Response<CarWashingDto>> AcceptOffer(Guid bookingId, Guid carWasherUserId, decimal offerAmount)
        {
            var response = new Response<CarWashingDto>();
            var carWasher = await _unitofWork.CarWasher.GetAsync(x => x.UserId == carWasherUserId);
            var booking = await _unitofWork.CarWashing.GetAsync(x => x.Id == bookingId);
            if (booking == null)
            {
                response.Message = "Booking not found";
                response.Success = false;
                return response;
            }

            booking.CarWasherId = carWasher.Id;
            booking.TotalPrice = offerAmount;
            booking.ServiceStatus = ServiceStatus.Booked;
            await _unitofWork.CarWashing.UpdateAsync(booking);
            response.Message = "Offer accepted successfully";
            response.Success = true;


            var res = await _mapServices.GetDistanceAsync(carWasher.AddressorLocation, booking.Location);
            var elem = res.rows.FirstOrDefault()?.elements.FirstOrDefault();
            var distanceToPickupLocation = elem?.distance.text;
            var durationToPickupLocation = elem?.duration.text;
            if (distanceToPickupLocation == null || durationToPickupLocation == null)
            {
                response.Message = "Failed to calculate distance or duration.";
                response.Success = false;
                return response;
            }
            response.Data = new CarWashingDto
            {
                CarWashingId = booking.Id,
                CarWasherId = booking.CarWasherId,
                CarBrand = booking.CarBrand,
                CarModel = booking.CarModel,
                TotalPrice = booking.TotalPrice,
                ServiceStatus = booking.ServiceStatus.ToString(),
                Location = booking.Location,
                DistancetoLocation = distanceToPickupLocation,
                DurationtoLocation = durationToPickupLocation,
                CarWashingItems = booking.CarWashingItems.Select(i => new CarWashingItemDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Price = i.Price
                }).ToList()
            };
            response.Success = true;
            response.Message = "Offer accepted successfully";
            return response;
        }

        public async Task<Response<CarWashingDto>> Book(Guid carWashingId)
        {
            var response = new Response<CarWashingDto>();
            var booking = await _unitofWork.CarWashing.GetAsync(x => x.Id ==  carWashingId);
            if (booking == null)
            {
                response.Message = "CarWashing Not found";
                response.Success = false;
                return response;
            }
            if(booking.CarWasherId != null)
            {
                booking.TotalPrice = booking.EstimatedPrice;
                booking.ServiceStatus = ServiceStatus.Booked;
                await _unitofWork.CarWashing.UpdateAsync(booking);
                response.Message = "Booked succesfully";
                response.Success = true;
                response.Data = new CarWashingDto
                {
                    CarWashingId = booking.Id,
                    CarWasherId = booking.CarWasherId,
                    CarBrand = booking.CarBrand,
                    CarModel = booking.CarModel,
                    TotalPrice = booking.TotalPrice,
                    ServiceStatus = booking.ServiceStatus.ToString(),
                    Location = booking.Location,
                    CarWashingItems = booking.CarWashingItems.Select(i => new CarWashingItemDto
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Price = i.Price
                    }).ToList()
                };
            
                return response;
            }
            else
            {
                booking.ServiceStatus = ServiceStatus.Pending;
                await _unitofWork.CarWashing.UpdateAsync(booking);
                response.Message = "Booked succesfully, awaiting for service provider ";
                response.Success = true;
                var carWashingDto = new CarWashingDto
                {
                    CarWashingId = booking.Id,
                    CarWasherId = booking.CarWasherId,
                    CarBrand = booking.CarBrand,
                    CarModel = booking.CarModel,
                    TotalPrice = booking.TotalPrice,
                    ServiceStatus = booking.ServiceStatus.ToString(),
                    Location = booking.Location,
                    CarWashingItems = booking.CarWashingItems.Select(i => new CarWashingItemDto
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Price = i.Price
                    }).ToList()
                };
                var not = await NotifyServiceProviders(carWashingDto);
                carWashingDto.ProvidersNotified = not.ProvidersNotified;
                response.Data = carWashingDto;
                return response;
            }
    
        }

        public async Task<Response<bool>> CancelBooking(Guid carWashingId)
        {
            var response = new Response<bool>();
            var carWashing = await _unitofWork.CarWashing.GetAsync(x => x.Id == carWashingId);
            if (carWashing == null)
            {
                response.Message = "CarWashing Not found";
                response.Success = false;
                return response;
            }
            if (carWashing.ServiceStatus == ServiceStatus.Done || carWashing.ServiceStatus == ServiceStatus.Cancelled)
            {
                response.Message = "Cannot cancel a completed or already cancelled booking.";
                response.Success = false;
                return response;
            }
            carWashing.ServiceStatus = ServiceStatus.Cancelled;
            await _unitofWork.CarWashing.UpdateAsync(carWashing);
            response.Message = "Booking cancelled successfully";
            response.Success = true;
            throw new NotImplementedException();
        }

        public async Task<Response<IEnumerable<CarWashingDto>>> GetBookings(Guid userId)
        {
            var response = new Response<IEnumerable<CarWashingDto>>();
            var carWashings = await _unitofWork.CarWashing.GetAllByExpressionAsync(x => x.CarWasher.UserId == userId || x.CustomerId == userId && x.ServiceStatus != ServiceStatus.Init);
            if (carWashings == null)
            {
                response.Message = "No bookings found for this car washer.";
                response.Success = false;
                return response;
            }
            response.Data = carWashings.Select(x => new CarWashingDto
            {
                CarWashingId = x.Id,
                CarWasherId = x.CarWasherId,
                CarBrand = x.CarBrand,
                CarModel = x.CarModel,
                TotalPrice = x.TotalPrice,
                ServiceStatus = x.ServiceStatus.ToString(),
                CarWashingItems = x.CarWashingItems.Select(i => new CarWashingItemDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Price = i.Price
                }).ToList()
            });
            response.Success = true;
            response.Message = "Bookings retrieved successfully.";
            return response;
        }

        public async Task<Response<IEnumerable<CarWashingDto>>> GetPendings(string location)
        {
            var response = new Response<IEnumerable<CarWashingDto>>();
            var carWashings = await _unitofWork.CarWashing.GetAllByExpressionAsync(x => x.ServiceStatus == ServiceStatus.Pending);
            if (carWashings == null || !carWashings.Any())
            {
                response.Message = "No pending bookings found.";
                response.Success = false;
                return response;
            }
            var bookings = new List<CarWashingDto>();
            foreach (var item in carWashings)
            {
                var distdura = await _mapServices.GetDistanceAndDurationAsync(location, item.Location);

                var result = await _mapServices.GetDistanceAsync(location, item.Location);
                var element = result.rows.FirstOrDefault()?.elements.FirstOrDefault();
                if (element == null || element.status != "OK")
                {
                    response.Message = "Failed to calculate distance and duration.";
                    response.Success = false;
                    return response;
                }
                var distance = element.distance.text;
                var duration = element.duration.text;
                if (distdura.DistanceMeters <= 50000) 
                {
                    bookings.Add(new CarWashingDto
                    {
                        CarWashingId = item.Id,
                        CarBrand = item.CarBrand,
                        CarModel = item.CarModel,
                        EstimatedPrice = item.EstimatedPrice,
                        ServiceStatus = item.ServiceStatus.ToString(),
                        Location = item.Location,
                        DistancetoLocation = distance,
                        DurationtoLocation = duration,
                        CarWashingItems = item.CarWashingItems.Select(i => new CarWashingItemDto
                        {
                            Id = i.Id,
                            Name = i.Name,
                            Price = i.Price
                        }).ToList()
                    });
                }
            }
            if (!bookings.Any())
            {
                response.Message = "No pending bookings found within the specified location.";
                response.Success = false;
                return response;
            }
            response.Data = bookings;
            response.Success = true;
            response.Message = "Pending bookings retrieved successfully.";
            return response;
        }

        public async Task<Response<CarWashingDto>> InitiateBooking(Guid customerId, CarWashingRequest request, Guid? carWasherUserId = null)
        {
            var response = new Response<CarWashingDto>();
            var carwasher = new CarWasher();
            var carWashing = new CarWashing();
            if (carWasherUserId != null)
            {
                carwasher = await _unitofWork.CarWasher.GetAsync(x => x.UserId == carWasherUserId);
                if (carwasher == null)
                {
                    response.Success = false;
                    response.Message = "Car washer not found.";
                    return response;
                }
                carWashing = new CarWashing
                {
                    CustomerId = customerId,
                    CarWasherId = carwasher.Id,
                    CarBrand = request.CarBrand,
                    CarModel = request.CarModel,
                    ServiceStatus = ServiceStatus.Init,
                    EstimatedPrice = 0,
                    CarWashingItems = new List<CarWashingitem>()
                };
            }
            else
            {
                //var distance = await _mapServices.GetDistanceAndDurationAsync(reques, item.Location);
                carWashing = new CarWashing
                {
                    CustomerId = customerId,
                    CarBrand = request.CarBrand,
                    CarModel = request.CarModel,
                    ServiceStatus = ServiceStatus.Init,
                    EstimatedPrice = 0,
                    Location = request.Location,
                    CarWashingItems = new List<CarWashingitem>()
                };
            }
            foreach(var item  in request.CarWashingItems)
            {
                var carWashItem = await _unitofWork.Context.CarWashItems.FirstOrDefaultAsync(x => x.Id == item.CarWashItemId);
                if (carWashItem == null)
                {
                    response.Success = false;
                    response.Message = $"Car wash item with ID {item.CarWashItemId} not found.";
                    return response;
                }
                var carwashingItem = new CarWashingitem
                {
                    CarWashingId = carWashing.Id,
                    Name = carWashItem.Name,
                    Price = carWashItem.Price,
                    CarWashItemId = item.CarWashItemId
                };

                carWashing.CarWashingItems.Add(carwashingItem);
                await _unitofWork.Context.SaveChangesAsync();
                carWashing.EstimatedPrice += carWashItem.Price;
            }

            await _unitofWork.CarWashing.AddAsync(carWashing);
            response.Message = "BookingInitiated";
            response.Success = true;
            response.Data = new CarWashingDto
            {
                CarWashingId = carWashing.Id,
                CustomerId = carWashing.CustomerId,
                CarWasherId = carWashing.CarWasherId,
                CarBrand = carWashing.CarBrand,
                CarModel = carWashing.CarModel,
                EstimatedPrice = carWashing.EstimatedPrice,
                Location = carWashing.Location,
                CarWashingItems = carWashing.CarWashingItems.Select(x => new CarWashingItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                }).ToList()
            };
            return response;
        }

        public async Task<Response<CarWashingDto>> RaisePrice(Guid bookingId, decimal newPrice)
        {
            var response = new Response<CarWashingDto>();
            var booking = await _unitofWork.CarWashing.GetAsync(x => x.Id == bookingId);
            if (booking == null)
            {
                response.Message = "Booking not found";
                response.Success = false;
                return response;
            }
            if (newPrice <= booking.EstimatedPrice)
            {
                response.Success = false;
                response.Message = "You can only increase the price.";
                return response;
            }
            booking.EstimatedPrice = newPrice;
            await _unitofWork.CarWashing.UpdateAsync(booking);
            response.Message = "Price raised successfully";
            response.Success = true;
            var data = new CarWashingDto
            {
                CarWashingId = booking.Id,
                CustomerId = booking.CustomerId,
                CarWasherId = booking.CarWasherId,
                CarBrand = booking.CarBrand,
                CarModel = booking.CarModel,
                EstimatedPrice = booking.EstimatedPrice,
                Location = booking.Location,
                ServiceStatus = booking.ServiceStatus.ToString(),
                CarWashingItems = booking.CarWashingItems.Select(i => new CarWashingItemDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Price = i.Price
                }).ToList()
            };
            var notify = await NotifyServiceProviders(response.Data);
            data.ProvidersNotified = notify.ProvidersNotified;
            response.Data = data;
            return response;
        }

        public async Task<Response<bool>> RejectBooking(Guid carWashingId)
        {
            var response = new Response<bool>();
            var carWashing = await _unitofWork.CarWashing.GetAsync(x => x.Id == carWashingId);
            if (carWashing == null)
            {
                response.Message = "CarWashing Not found";
                response.Success = false;
                return response;
            }
            if (carWashing.ServiceStatus == ServiceStatus.Done || carWashing.ServiceStatus == ServiceStatus.Cancelled)
            {
                response.Message = "Cannot reject a completed or already cancelled booking.";
                response.Success = false;
                return response;
            }
            carWashing.ServiceStatus = ServiceStatus.Rejected;
            await _unitofWork.CarWashing.UpdateAsync(carWashing);
            response.Message = "Booking rejected successfully";
            response.Success = true;
            response.Data = true;
            return response;
        }




    }
}
