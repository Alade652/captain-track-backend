using CaptainTrackBackend.Application.Abstraction.Interface.Maps;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.WaterSupply;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders.CarWash;
using CaptainTrackBackend.Application.DTO.ServiceProviders.WaterSupply;
using CaptainTrackBackend.Application.Services.ServiceProviders.VehicleTowing;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.VehicleTowing;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.WaterSupply;
using CaptainTrackBackend.Domain.Enum;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Services.ServiceProviders.WaterSupply
{
    public class WaterSupplingService : IWaterSupplingService
    {
        private readonly IUnitofWork _unitOfWork;
        private readonly IMapServices _mapServices;
        private readonly IHubContext<NegotiationHub> _hubContext;
        private readonly ILogger<WaterSupplingService> _logger;

        public WaterSupplingService(IUnitofWork unitOfWork, IMapServices mapServices, IHubContext<NegotiationHub> hubContext, ILogger<WaterSupplingService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapServices = mapServices;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task<NotificationResult> NotifyServiceProviders(WaterSupplingDto booking)
        {
            var response = new NotificationResult();
            try
            {
                var providers = await _unitOfWork.WaterSupplier.GetAllByExpression(x => x.ServiceProviderRole == ServiceProviderRole.Freelancer && x.IsAvailable == true);
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
                        var distdura = await _mapServices.GetDistanceAndDurationAsync(provider.AddressorLocation, booking.DeliveryAddress);
                        var result = await _mapServices.GetDistanceAsync(provider.AddressorLocation, booking.DeliveryAddress);
                        var element = result.rows.FirstOrDefault()?.elements.FirstOrDefault();
                        if (element == null || element.status != "OK")
                        {
                            _logger.LogWarning($"Failed to calculate distance");
                            continue;
                        }

                        if (element != null && element.status == "OK" && distdura.DistanceMeters <= 5000)
                        {

                            booking.DistancetoLocation = element.distance.text;
                            booking.DuraiontoLocation = element.duration.text;


                            await _hubContext.Clients.User(provider.Id.ToString())
                                .SendAsync("ReceivePendingBooking", booking);
                            response.ProvidersNotified++;
                            _logger.LogInformation($"Notified provider {provider.Id} f");
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
                _logger.LogError(ex, "Error in NotifyServiceProvidersAsync for booking ID {BookingId}", booking.WaterSupplingId);
            }

            return response;
        }


        public async Task<Response<bool>> AcceptBooking(Guid waterSupplingId)
        {
            var response = new Response<bool>();
            var booking = await _unitOfWork.WaterSuppling.GetAsync(x => x.Id == waterSupplingId);
            if (booking == null)
            {
                response.Success = false;
                response.Message = "Booking not found.";
                return response;
            }
            booking.DeliveryStatus = ServiceStatus.Booked;
            await _unitOfWork.WaterSuppling.UpdateAsync(booking);
            response.Success = true;
            response.Message = "Booking Booked successfully.";
            return response;
        }

        public async Task<Response<WaterSupplingDto>> AcceptOffer(Guid bookingId, Guid waterSupplierUserId, decimal offerAmount)
        {
            var response = new Response<WaterSupplingDto>();
            var booking = await _unitOfWork.WaterSuppling.GetAsync(x => x.Id == bookingId);
            if (booking == null)
            {
                response.Message = "Booking not found";
                response.Success = false;
                return response;
            }
            var waterSupplier = await _unitOfWork.WaterSupplier.GetAsync(x => x.UserId == waterSupplierUserId);

            booking.WaterSupplierId = waterSupplier.Id;
            booking.TotalPrice = offerAmount;
            booking.DeliveryStatus = ServiceStatus.Booked;
            await _unitOfWork.WaterSuppling.UpdateAsync(booking);


            var res = await _mapServices.GetDistanceAsync(waterSupplier.AddressorLocation, booking.DeliveryAddress);
            var elem = res.rows.FirstOrDefault()?.elements.FirstOrDefault();
            var distanceToPickupLocation = elem?.distance.text;
            var durationToPickupLocation = elem?.duration.text;
            if (distanceToPickupLocation == null || durationToPickupLocation == null)
            {
                response.Message = "Failed to calculate distance or duration.";
                response.Success = false;
                return response;
            }

            response.Message = "Offer accepted successfully";
            response.Success = true;
            response.Data = new WaterSupplingDto
            {
                WaterSupplingId = booking.Id,
                WaterSupplierId = booking.WaterSupplierId,
                CustomerId = booking.CustomerId,
                QuantityInLitres = booking.QuantityInLitres,
                DeliveryDate = booking.DeliveryDate,
                DeliveryAddress = booking.DeliveryAddress,
                DistancetoLocation = booking.DistancetoLocation,
                DuraiontoLocation = booking.DuraiontoLocation,  
                EstimateTotalAmount = booking.EstimateTotalAmount,
                TotalPrice = booking.TotalPrice,
                DeliveryStatus = booking.DeliveryStatus.ToString()
            };
            return response;

        }

        public async Task<Response<WaterSupplingDto>> Book(Guid waterSupplingId)
        {
            var response = new Response<WaterSupplingDto>();
            var booking = await _unitOfWork.WaterSuppling.GetAsync(x => x.Id == waterSupplingId);
            if (booking == null)
            {
                response.Message = "Booking not found.";
                response.Success = false;
                return response;
            }
            if (booking.WaterSupplierId != null)
            {
                booking.DeliveryStatus = ServiceStatus.Booked;
                booking.EstimateTotalAmount = booking.TotalPrice;
                await _unitOfWork.WaterSuppling.UpdateAsync(booking);
                response.Data = new WaterSupplingDto
                {
                    WaterSupplingId = booking.Id,
                    WaterSupplierId = booking.WaterSupplierId,
                    CustomerId = booking.CustomerId,
                    QuantityInLitres = booking.QuantityInLitres,
                    DeliveryDate = booking.DeliveryDate,
                    DeliveryAddress = booking.DeliveryAddress,
                    DistancetoLocation = booking.DistancetoLocation,
                    DuraiontoLocation = booking.DuraiontoLocation,
                    EstimateTotalAmount = booking.EstimateTotalAmount,
                    TotalPrice = booking.TotalPrice,
                    DeliveryStatus = booking.DeliveryStatus.ToString()
                };
                response.Success = true;
                response.Message = "Booking confirmed successfully.";
                return response;
            }
            else
            {
                booking.DeliveryStatus = ServiceStatus.Pending;
                await _unitOfWork.WaterSuppling.UpdateAsync(booking);
                response.Success = true;
                response.Message = "Booking confirmed successfully. awaiting service provider";
                var data = new WaterSupplingDto
                {
                    WaterSupplingId = booking.Id,
                    WaterSupplierId = booking.WaterSupplierId,
                    CustomerId = booking.CustomerId,
                    QuantityInLitres = booking.QuantityInLitres,
                    DeliveryDate = booking.DeliveryDate,
                    DeliveryAddress = booking.DeliveryAddress,
                    EstimateTotalAmount = booking.EstimateTotalAmount,
                    TotalPrice = booking.TotalPrice,
                    DeliveryStatus = booking.DeliveryStatus.ToString()
                };
                var notificationResult = await NotifyServiceProviders(data);
                if (!notificationResult.Success)
                {
                    response.Message = notificationResult.Message;
                    response.Success = false;
                    return response;
                }
                data.ProvidersNotified = notificationResult.ProvidersNotified;
                response.Data = data;
                return response;
            }

        }

        public async Task<Response<bool>> CancelBooking(Guid waterSupplingId)
        {
            var response = new Response<bool>();
            var booking = await _unitOfWork.WaterSuppling.GetAsync(x => x.Id == waterSupplingId);
            if (booking == null)
            {
                response.Message = "Booking not found.";
                response.Success = false;
                return response;
            }
            booking.DeliveryStatus = ServiceStatus.Cancelled;
            await _unitOfWork.WaterSuppling.UpdateAsync(booking);
            response.Data = true;
            response.Success = true;
            response.Message = "Booking cancelled successfully.";
            return response;

        }

        public async Task<Response<WaterSupplingDto>> GetBookingById(Guid waterSupplingId)
        {
            var response = new Response<WaterSupplingDto>();
            var booking = await _unitOfWork.WaterSuppling.GetAsync(x => x.Id == waterSupplingId);
            if (booking == null)
            {
                response.Message = "Booking not found.";
                response.Success = false;
                return response;
            }

            response.Data = new WaterSupplingDto
            {
                WaterSupplingId = booking.Id,
                WaterSupplierId = booking.WaterSupplierId,
                CustomerId = booking.CustomerId,
                QuantityInLitres = booking.QuantityInLitres,
                DeliveryDate = booking.DeliveryDate,
                DeliveryAddress = booking.DeliveryAddress,
                DistancetoLocation = booking.DistancetoLocation,
                DuraiontoLocation = booking.DuraiontoLocation,
                EstimateTotalAmount = booking.EstimateTotalAmount,
                TotalPrice = booking.TotalPrice,
                DeliveryStatus = booking.DeliveryStatus.ToString()
            };

            response.Success = true;
            response.Message = "Booking retrieved successfully.";
            return response;
        }

        public async Task<Response<IEnumerable<WaterSupplingDto>>> GetBookings(Guid userId)
        {
            var response = new Response<IEnumerable<WaterSupplingDto>>();
            var waterSupplings = await _unitOfWork.WaterSuppling.GetBookings(x => x.WaterSupplier.UserId == userId ||  x.CustomerId == userId && x.DeliveryStatus != ServiceStatus.Init);
            if (waterSupplings == null)
            {
                response.Success = false;
                response.Message = "No bookings";
                return response;
            }


            response.Success = true;
            response.Message = "Bookings retrieved successfully.";
            response.Data = waterSupplings.Select(booking => new WaterSupplingDto
            {
                WaterSupplingId = booking.Id,
                WaterSupplierId = booking.WaterSupplierId,
                CustomerId = booking.CustomerId,
                QuantityInLitres = booking.QuantityInLitres,
                DeliveryDate = booking.DeliveryDate,
                DeliveryAddress = booking.DeliveryAddress,
                DistancetoLocation = booking.DistancetoLocation,
                DuraiontoLocation = booking.DuraiontoLocation,
                EstimateTotalAmount = booking.EstimateTotalAmount,
                TotalPrice = booking.TotalPrice,
                DeliveryStatus = booking.DeliveryStatus.ToString()
            });
            return response;
        }

        public async Task<Response<IEnumerable<WaterSupplingDto>>> GetPending(string location)
        {
            var response = new Response<IEnumerable<WaterSupplingDto>>();
            var bookings = await _unitOfWork.WaterSuppling.GetAllByExpression(x => x.DeliveryStatus == ServiceStatus.Pending);
            if (bookings == null || !bookings.Any())
            {
                response.Message = "No pending bookings found.";
                response.Success = false;
                return response;
            }

            var pendingbookings = new List<WaterSupplingDto>();
            foreach (var booking in bookings)
            {
                var distandDura = await _mapServices.GetDistanceAndDurationAsync(location, booking.DeliveryAddress);

                var result = await _mapServices.GetDistanceAsync(location, booking.DeliveryAddress);
                var element = result.rows.FirstOrDefault()?.elements.FirstOrDefault();
                if (element == null || element.status != "OK")
                {
                    response.Message = "Failed to calculate distance and duration.";
                    response.Success = false;
                    return response;
                }
                var distance = element.distance.text;
                var duration = element.duration.text;

                if (distandDura.DistanceMeters <= 3000)
                {
                    pendingbookings.Add(new WaterSupplingDto
                    {
                        WaterSupplingId = booking.Id,
                        WaterSupplierId = booking.WaterSupplierId,
                        CustomerId = booking.CustomerId,
                        QuantityInLitres = booking.QuantityInLitres,
                        DeliveryDate = booking.DeliveryDate,
                        DeliveryAddress = booking.DeliveryAddress,
                        DistancetoLocation = distance,
                        DuraiontoLocation = duration,
                        EstimateTotalAmount = booking.EstimateTotalAmount,
                        TotalPrice = booking.TotalPrice,
                        DeliveryStatus = booking.DeliveryStatus.ToString()
                    });
                }
            }
            if (pendingbookings == null)
            {
                response.Message = "No pending bookings found within the specified location.";
                response.Success = false;
                return response;
            }

            response.Message = "Pending Bookings gotten";
            response.Success = true;
            response.Data = pendingbookings;
            return response;

        }

        public async Task<Response<WaterSupplingDto>> InitBooking(WaterSupplingRequest request, Guid customerId, Guid? waterSupplierUserId = null)
        {
            var response = new Response<WaterSupplingDto>();

            WaterSuppling waterSuppling = new WaterSuppling();
            if (waterSupplierUserId != null)
            {
                var waterSupplier = await _unitOfWork.WaterSupplier.GetAsync(x => x.UserId == waterSupplierUserId);
                if (waterSupplier == null)
                {
                    response.Message = "Water supplier not found.";
                    response.Success = false;
                    return response;
                }

                var res = await _mapServices.GetDistanceAsync(waterSupplier.AddressorLocation, request.DeliveryAddress);
                var elem = res.rows.FirstOrDefault()?.elements.FirstOrDefault();
                var distanceToPickupLocation = elem?.distance.text;
                var durationToPickupLocation = elem?.duration.text;
                if (distanceToPickupLocation == null || durationToPickupLocation == null)
                {
                    response.Message = "Failed to calculate distance or duration.";
                    response.Success = false;
                    return response;
                }


                waterSuppling.WaterSupplierId = waterSupplier.Id;
                waterSuppling.CustomerId = customerId;
                waterSuppling.DeliveryAddress = request.DeliveryAddress;
                waterSuppling.QuantityInLitres = request.QuantityInLitres;
                waterSuppling.DeliveryDate = request.DeliveryDate;
                waterSuppling.DistancetoLocation = distanceToPickupLocation;
                waterSuppling.DuraiontoLocation = durationToPickupLocation;
                waterSuppling.DeliveryStatus = ServiceStatus.Init;
                waterSuppling.TotalPrice = waterSupplier.PricePerLitre * request.QuantityInLitres;

            }
            else
            {
                var prices = await _unitOfWork.Context.Prices.FirstOrDefaultAsync();

                waterSuppling.CustomerId = customerId;
                waterSuppling.DeliveryAddress = request.DeliveryAddress;
                waterSuppling.QuantityInLitres = request.QuantityInLitres;
                waterSuppling.DeliveryDate = request.DeliveryDate;
                waterSuppling.DeliveryStatus = ServiceStatus.Init;
                waterSuppling.EstimateTotalAmount = request.QuantityInLitres * prices.WaterPricePerLitre;
            }

            await _unitOfWork.WaterSuppling.AddAsync(waterSuppling);

            response.Data = new WaterSupplingDto
            {
                WaterSupplingId = waterSuppling.Id,
                WaterSupplierId = waterSuppling.WaterSupplierId,
                CustomerId = waterSuppling.CustomerId,
                QuantityInLitres = waterSuppling.QuantityInLitres,
                DeliveryDate = waterSuppling.DeliveryDate,
                DeliveryAddress = waterSuppling.DeliveryAddress,
                DistancetoLocation = waterSuppling.DistancetoLocation,
                DuraiontoLocation = waterSuppling.DuraiontoLocation,
                EstimateTotalAmount = waterSuppling.EstimateTotalAmount,
                TotalPrice = waterSuppling.TotalPrice,
                DeliveryStatus = waterSuppling.DeliveryStatus.ToString()
            };
            response.Success = true;
            response.Message = "Booking initialized successfully.";
            return response;
        }

        public async Task<Response<WaterSupplingDto>> RaisePrice(Guid bookingId, decimal newPrice)
        {
            var response = new Response<WaterSupplingDto>();
            var booking = await _unitOfWork.WaterSuppling.GetAsync(x => x.Id == bookingId);
            if (booking == null)
            {
                response.Message = "Booking not found";
                response.Success = false;
                return response;
            }
            if (newPrice <= booking.EstimateTotalAmount)
            {
                response.Success = false;
                response.Message = "You can only increase the price.";
                return response;
            }
            booking.EstimateTotalAmount = newPrice;
            await _unitOfWork.WaterSuppling.UpdateAsync(booking);
            response.Message = "Price raised successfully";
            response.Success = true;
            var data = new WaterSupplingDto
            {
                WaterSupplingId = booking.Id,
                WaterSupplierId = booking.WaterSupplierId,
                CustomerId = booking.CustomerId,
                QuantityInLitres = booking.QuantityInLitres,
                DeliveryDate = booking.DeliveryDate,
                DeliveryAddress = booking.DeliveryAddress,
                DistancetoLocation = booking.DistancetoLocation,
                DuraiontoLocation = booking.DuraiontoLocation,
                EstimateTotalAmount = booking.EstimateTotalAmount,
                TotalPrice = booking.TotalPrice,
                DeliveryStatus = booking.DeliveryStatus.ToString()
            };
            var notify = await NotifyServiceProviders(response.Data);
            data.ProvidersNotified = notify.ProvidersNotified;
            response.Data = data;
            return response;
        }

        public async Task<Response<bool>> RejectBooking(Guid waterSupplingId)
        {
            var response = new Response<bool>();
            var booking = await _unitOfWork.WaterSuppling.GetAsync(x => x.Id == waterSupplingId);
            if (booking == null)
            {
                response.Success = false;
                response.Message = "Booking not found.";
                return response;
            }
            booking.DeliveryStatus = ServiceStatus.Booked;
            await _unitOfWork.WaterSuppling.UpdateAsync(booking);
            response.Success = true;
            response.Message = "Booking Booked successfully.";
            return response;
        }
    }
}
