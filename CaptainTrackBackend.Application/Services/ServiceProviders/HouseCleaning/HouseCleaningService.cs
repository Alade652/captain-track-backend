using CaptainTrackBackend.Application.Abstraction.Interface.Maps;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.HouseCleaning;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.CarWash;
using CaptainTrackBackend.Application.DTO.ServiceProviders.HouseCleaning;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.DryCleaning;
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

namespace CaptainTrackBackend.Application.Services.ServiceProviders.HouseCleaning
{
    public class HouseCleaningService : IHouseCleaningService
    {
        private readonly IUnitofWork _unitOfWork;
        private readonly IMapServices _mapServices;
        private readonly IHubContext<NegotiationHub> _hubContext;
        private readonly ILogger<HouseCleaningService> _logger;

        public HouseCleaningService(IUnitofWork unitOfWork, IMapServices mapServices,
            IHubContext<NegotiationHub> hubContext, ILogger<HouseCleaningService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapServices = mapServices;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task<NotificationResult> NotifyServiceProviders(HouseCleaningDto booking)
        {
            var response = new NotificationResult();
            try
            {
                var providers = await _unitOfWork.HouseCleaner.GetAllByExpression(x => x.ServiceProviderRole == ServiceProviderRole.Freelancer && x.IsAvailable == true);
                if (providers == null || !providers.Any())
                {
                    response.Success = false;
                    response.Message = $"No service providers availabe";
                    _logger.LogWarning(response.Message);
                    return response;
                }
                foreach (var provider in providers)
                {
                    try
                    {
                        var distdura = await _mapServices.GetDistanceAndDurationAsync(provider.AddressorLocation, booking.AddressorLocation);
                        var result = await _mapServices.GetDistanceAsync(provider.AddressorLocation, booking.AddressorLocation);
                        var element = result.rows.FirstOrDefault()?.elements.FirstOrDefault();
                        if (element == null || element.status != "OK")
                        {
                            _logger.LogWarning($"Failed to calculate distance");
                            continue;
                        }

                        if (element != null && element.status == "OK" && distdura.DistanceMeters <= 5000)
                        {

                            booking.DistancetoLocation = element.distance.text;
                            booking.DistancetoLocation = element.duration.text;


                            await _hubContext.Clients.User(provider.Id.ToString())
                                .SendAsync("ReceivePendingBooking", booking);
                            response.ProvidersNotified++;
                            _logger.LogInformation($"Notified provider {provider.Id} ");
                        }
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
                _logger.LogError(ex, "Error in NotifyServiceProvidersAsync for booking ID {BookingId}", booking.HouseCleanerId);
            }

            return response;
        }


        public async Task<Response<HouseCleaningDto>> Book(Guid houseCleaningid)
        {
            var response = new Response<HouseCleaningDto>();
            var houseCleaning = await _unitOfWork.HouseCleaning.GetAsync(x => x.Id == houseCleaningid);
            if (houseCleaning == null)
            {
                response.Message = "House cleaning not found.";
                response.Success = false;
                return response;
            }

            if(houseCleaning.HouseCleanerId != null)
            {
                houseCleaning.ServiceStatus = ServiceStatus.Booked;
                houseCleaning.TotalPrice = houseCleaning.EstimatedPrice;
                await _unitOfWork.HouseCleaning.UpdateAsync(houseCleaning);
                response.Message = "House cleaning booked Succesfully.";
                response.Success = true;
                response.Data = new HouseCleaningDto
                {
                    HouseCleaningId = houseCleaning.Id,
                    CustomerId = houseCleaning.CustomerId,
                    HouseCleanerId = houseCleaning.HouseCleanerId,
                    HouseCleanerPackageId = houseCleaning.HouseCleanerPackageId,
                    AddressorLocation = houseCleaning.AddressorLocation,
                    ServiceDate = houseCleaning.ServiceDate,
                    TotalPrice = houseCleaning.TotalPrice,
                    ServiceStatus = houseCleaning.ServiceStatus.ToString(),
                    HouseCleanerItems = houseCleaning.HouseCleaningItems.Select(i => new HouseCleaningItemDto
                    {
                        HouseCleaningItemId = i.Id,
                        Name = i.Name,
                        ItemsPrice = i.ItemsPrice,
                        Quantity = i.Quantity
                    }).ToList()
                };
                return response;
            }
            else
            {
                houseCleaning.ServiceStatus = ServiceStatus.Pending;
                await _unitOfWork.HouseCleaning.UpdateAsync(houseCleaning);
                response.Message = "House cleaning is pending for a house cleaner to accept the booking.";
                response.Success = true;
                var data = new HouseCleaningDto
                {
                    HouseCleaningId = houseCleaning.Id,
                    CustomerId = houseCleaning.CustomerId,
                    HouseCleanerId = houseCleaning.HouseCleanerId,
                    HouseCleanerPackageId = houseCleaning.HouseCleanerPackageId,
                    AddressorLocation = houseCleaning.AddressorLocation,
                    ServiceDate = houseCleaning.ServiceDate,
                    EstimatePrice = houseCleaning.EstimatedPrice,
                    ServiceStatus = houseCleaning.ServiceStatus.ToString(),
                    HouseCleanerItems = houseCleaning.HouseCleaningItems.Select(i => new HouseCleaningItemDto
                    {
                        HouseCleaningItemId = i.Id,
                        Name = i.Name,
                        ItemsPrice = i.ItemsPrice,
                        Quantity = i.Quantity
                    }).ToList()
                };
                var notificationResult = await NotifyServiceProviders(data);
                data.ProvidersNotified = notificationResult.ProvidersNotified;
                response.Data = data;
                return response;
            }
        }

        public async Task<Response<List<HouseCleaningDto>>> GetHouseCleanings(Guid userId)
        {
            var response = new Response<List<HouseCleaningDto>>();
            var houseCleanings = await _unitOfWork.HouseCleaning.GetAllByExpressionAsync(x => x.HouseCleaner.UserId == userId || x.CustomerId == userId && x.ServiceStatus != ServiceStatus.Init);
            if (houseCleanings == null || !houseCleanings.Any())
            {
                response.Message = "No house cleanings found for this house cleaner.";
                response.Success = true;
                return response;
            }
            response.Data = houseCleanings.Select(x => new HouseCleaningDto
            {
                HouseCleaningId = x.Id,
                CustomerId = x.CustomerId,
                HouseCleanerId = x.HouseCleanerId,
                HouseCleanerPackageId = x.HouseCleanerPackageId,
                AddressorLocation = x.AddressorLocation,
                ServiceDate = x.ServiceDate,
                EstimatePrice = x.EstimatedPrice,
                ServiceStatus = x.ServiceStatus.ToString(),
                HouseCleanerItems = x.HouseCleaningItems.Select(i => new HouseCleaningItemDto
                {
                    HouseCleaningItemId = i.Id,
                    Name = i.Name,
                    ItemsPrice = i.ItemsPrice,
                    Quantity = i.Quantity
                }).ToList()
            }).ToList();
            response.Message = "House cleanings retrieved successfully.";
            response.Success = true;
            return response;
        }

        public async Task<Response<List<HouseCleaningDto>>> GetPending(string location)
        {
            var response = new Response<List<HouseCleaningDto>>();
            var houseCleanings = await _unitOfWork.HouseCleaning.GetAllByExpressionAsync
                (x => x.ServiceStatus == ServiceStatus.Pending && x.HouseCleanerId == null);
            if (houseCleanings == null || !houseCleanings.Any())
            {
                response.Message = "No pending house cleanings found.";
                response.Success = true;
                return response;
            }
            var bookings = new List<HouseCleaningDto>();
            foreach (var x in houseCleanings)
            {
                var distandDura = await _mapServices.GetDistanceAndDurationAsync(location, x.AddressorLocation);

                var result = await _mapServices.GetDistanceAsync(location, x.AddressorLocation);
                var element = result.rows.FirstOrDefault()?.elements.FirstOrDefault();
                if (element == null || element.status != "OK")
                {
                    response.Message = "Failed to calculate distance and duration.";
                    response.Success = false;
                    return response;
                }
                var distance = element.distance.text;
                var duration = element.duration.text;
                if (distandDura.DistanceMeters < 5000)
                {
                    bookings.Add(new HouseCleaningDto
                    {
                        HouseCleaningId = x.Id,
                        CustomerId = x.CustomerId,
                        HouseCleanerId = x.HouseCleanerId,
                        HouseCleanerPackageId = x.HouseCleanerPackageId,
                        AddressorLocation = x.AddressorLocation,
                        ServiceDate = x.ServiceDate,
                        EstimatePrice = x.EstimatedPrice,
                        ServiceStatus = x.ServiceStatus.ToString(),
                        DistancetoLocation = distance,
                        DuraiontoLocation = distance,
                        HouseCleanerItems = x.HouseCleaningItems.Select(i => new HouseCleaningItemDto
                        {
                            HouseCleaningItemId = i.Id,
                            Name = i.Name,
                            ItemsPrice = i.ItemsPrice,
                            Quantity = i.Quantity
                        }).ToList()
                    });
                }
            }

            response.Data = bookings;
            response.Message = "Pending house cleanings retrieved successfully.";
            response.Success = true;
            return response;
        }

        public async Task<Response<HouseCleaningDto>> InitBooking(Guid customerId, HouseCleaningRequest request, Guid? houseCleanerUserId = null)
        {
            var response = new Response<HouseCleaningDto>();
            HouseCleaner houseCleaner = null;
            if (houseCleanerUserId != null)
            {
                houseCleaner = await _unitOfWork.HouseCleaner.GetAsync(x => x.UserId == houseCleanerUserId);
                if(houseCleaner == null)
                {
                    response.Message = "House cleaner not found.";
                    response.Success = false;
                    return response;
                }
            }
            var houseCleaning = new Housecleaning();
            if(houseCleaner != null)
            {

                houseCleaning.CustomerId = customerId;
                houseCleaning.HouseCleanerId = houseCleaner.Id;
                houseCleaning.ServiceDate = request.ServiceDate;
                houseCleaning.AddressorLocation = request.AddressorLocation;
                houseCleaning.HouseCleanerPackageId = request.HouseCleanerPackageId;
                houseCleaning.EstimatedPrice = 0;
                houseCleaning.ServiceStatus = ServiceStatus.Init;
                houseCleaning.HouseCleaningItems = new List<HouseCleaningItem>();
                
            }
            else
            {
                houseCleaning.CustomerId = customerId;
                houseCleaning.ServiceDate = request.ServiceDate;
                houseCleaning.AddressorLocation = request.AddressorLocation;
                houseCleaning.HouseCleanerPackageId = request.HouseCleanerPackageId;
                houseCleaning.EstimatedPrice = 0;
                houseCleaning.ServiceStatus = ServiceStatus.Init;
                houseCleaning.HouseCleaningItems = new List<HouseCleaningItem>();
            }

            foreach (var item in request.HouseCleaningItems)
            {
                var houseCleanerItem = await _unitOfWork.Context.HouseCleanerItems.FirstOrDefaultAsync(x => x.Id == item.HouseCleanerItemId);
                if (houseCleanerItem == null)
                {
                    response.Message = $"House cleaner item with ID {item.HouseCleanerItemId} not found.";
                    response.Success = false;
                    return response;
                }
                var houseCleaningItem = new HouseCleaningItem
                {
                    HouseCleanerItemid = houseCleanerItem.Id,
                    HouseCleaningId = houseCleaning.Id,
                    Name = houseCleanerItem.Name,
                    Quantity = item.Quantity,
                    ItemsPrice = houseCleanerItem.Price * item.Quantity
                };
                houseCleaning.HouseCleaningItems.Add(houseCleaningItem);
                houseCleaning.EstimatedPrice += houseCleaningItem.ItemsPrice;
            }
            await _unitOfWork.HouseCleaning.AddAsync(houseCleaning);
            var houseCleaningDto = new HouseCleaningDto
            {
                HouseCleaningId = houseCleaning.Id,
                CustomerId = houseCleaning.CustomerId,
                HouseCleanerId = houseCleaning.HouseCleanerId,
                HouseCleanerPackageId = houseCleaning.HouseCleanerPackageId,
                AddressorLocation = houseCleaning.AddressorLocation,
                ServiceDate = houseCleaning.ServiceDate,
                EstimatePrice = houseCleaning.EstimatedPrice,
                ServiceStatus = houseCleaning.ServiceStatus.ToString(),
                HouseCleanerItems = houseCleaning.HouseCleaningItems.Select(x => new HouseCleaningItemDto
                {
                    HouseCleaningItemId = x.Id,
                    Name = x.Name,
                    ItemsPrice = x.ItemsPrice,
                    Quantity = x.Quantity
                }).ToList()
            };
            response.Data = houseCleaningDto;
            response.Message = "Booking initialized successfully.";
            response.Success = true;
            return response;
        }

        public async Task<Response<bool>> AcceptBooking(Guid houseCleaningId)
        {
            var response = new Response<bool>();
            var booking = await _unitOfWork.HouseCleaning.GetAsync(x => x.Id == houseCleaningId);
            if (booking == null)
            {
                response.Success = false;
                response.Message = "Booking not found.";
                return response;
            }
            booking.ServiceStatus = ServiceStatus.Booked;
            await _unitOfWork.HouseCleaning.UpdateAsync(booking);
            response.Success = true;
            response.Message = "Booking accepted successfully.";
            return response;
        }

        public async Task<Response<bool>> CancelBooking(Guid houseCleaningId)
        {
            var response = new Response<bool>();
            var booking = await _unitOfWork.HouseCleaning.GetAsync(x => x.Id == houseCleaningId);
            if (booking == null)
            {
                response.Success = false;
                response.Message = "Booking not found.";
                return response;
            }
            booking.ServiceStatus = ServiceStatus.Cancelled;
            await _unitOfWork.HouseCleaning.UpdateAsync(booking);
            response.Success = true;
            response.Message = "Booking Cancelled successfully.";
            return response;
        }

        public async Task<Response<bool>> RejectBooking(Guid houseCleaningId)
        {

            var response = new Response<bool>();
            var booking = await _unitOfWork.HouseCleaning.GetAsync(x => x.Id == houseCleaningId);
            if (booking == null)
            {
                response.Success = false;
                response.Message = "Booking not found.";
                return response;
            }
            booking.ServiceStatus = ServiceStatus.Booked;
            await _unitOfWork.HouseCleaning.UpdateAsync(booking);
            response.Success = true;
            response.Message = "Booking rejected successfully.";
            return response;
        }

        public async Task<Response<HouseCleaningDto>> AcceptOffer(Guid bookingId, Guid houseCleanerUserId, decimal offerAmount)
        {
            var response = new Response<HouseCleaningDto>(); 
            var booking = await _unitOfWork.HouseCleaning.GetAsync(x => x.Id == bookingId);
            if (booking == null)
            {
                response.Message = "Booking not found";
                response.Success = false;
                return response;
            }
            var houseCleaner = await _unitOfWork.HouseCleaner.GetAsync(x => x.UserId == houseCleanerUserId);
            if (houseCleaner == null)
            {
                response.Message = "House cleaner not found";
                response.Success = false;
                return response;
            }

            booking.HouseCleanerId = houseCleaner.Id;
            booking.TotalPrice = offerAmount;
            booking.ServiceStatus = ServiceStatus.Booked;
            await _unitOfWork.HouseCleaning.UpdateAsync(booking);
            response.Message = "Offer accepted successfully";
            response.Success = true;


            var res = await _mapServices.GetDistanceAsync(houseCleaner.AddressorLocation, booking.Location);
            var elem = res.rows.FirstOrDefault()?.elements.FirstOrDefault();
            var distanceToPickupLocation = elem?.distance.text;
            var durationToPickupLocation = elem?.duration.text;
            if (distanceToPickupLocation == null || durationToPickupLocation == null)
            {
                response.Message = "Failed to calculate distance or duration.";
                response.Success = false;
                return response;
            }

            response.Success = true;
            response.Message = "Offer accepted successfully.";
            response.Data = new HouseCleaningDto
            {
                HouseCleaningId = booking.Id,
                CustomerId = booking.CustomerId,
                HouseCleanerId = booking.HouseCleanerId,
                HouseCleanerPackageId = booking.HouseCleanerPackageId,
                AddressorLocation = booking.AddressorLocation,
                ServiceDate = booking.ServiceDate,
                EstimatePrice = booking.EstimatedPrice,
                TotalPrice = booking.TotalPrice,
                ServiceStatus = booking.ServiceStatus.ToString(),
                DistancetoLocation = distanceToPickupLocation,
                DuraiontoLocation = durationToPickupLocation,
                HouseCleanerItems = booking.HouseCleaningItems.Select(i => new HouseCleaningItemDto
                {
                    HouseCleaningItemId = i.Id,
                    Name = i.Name,
                    ItemsPrice = i.ItemsPrice,
                    Quantity = i.Quantity
                }).ToList()
            };
            return response;
        }

        public async Task<Response<HouseCleaningDto>> RaisePrice(Guid bookingId, decimal newPrice)
        {
            var response = new Response<HouseCleaningDto>();
            var booking = await _unitOfWork.HouseCleaning.GetAsync(x => x.Id == bookingId);
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
            await _unitOfWork.HouseCleaning.UpdateAsync(booking);
            response.Message = "Price raised successfully";
            response.Success = true;
            var data = new HouseCleaningDto
            {
                HouseCleaningId = booking.Id,
                CustomerId = booking.CustomerId,
                HouseCleanerId = booking.HouseCleanerId,
                HouseCleanerPackageId = booking.HouseCleanerPackageId,
                AddressorLocation = booking.AddressorLocation,
                ServiceDate = booking.ServiceDate,
                EstimatePrice = booking.EstimatedPrice,
                TotalPrice = booking.TotalPrice,
                ServiceStatus = booking.ServiceStatus.ToString(),
                HouseCleanerItems = booking.HouseCleaningItems.Select(i => new HouseCleaningItemDto
                {
                    HouseCleaningItemId = i.Id,
                    Name = i.Name,
                    ItemsPrice = i.ItemsPrice,
                    Quantity = i.Quantity
                }).ToList()
            };
            var notify = await NotifyServiceProviders(response.Data);
            data.ProvidersNotified = notify.ProvidersNotified;
            response.Data = data;
            return response;
        }
    }
}
