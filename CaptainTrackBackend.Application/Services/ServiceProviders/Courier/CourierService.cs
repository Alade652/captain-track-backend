using CaptainTrackBackend.Application.Abstraction.Interface.Maps;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.Courier;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.Ambulance;
using CaptainTrackBackend.Application.DTO.ServiceProviders.CarWash;
using CaptainTrackBackend.Application.DTO.ServiceProviders.Courier;
using CaptainTrackBackend.Domain.Entities;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.Ambulance;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.CarWash;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.Courier;
using CaptainTrackBackend.Domain.Enum;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Services.ServiceProviders.Courier
{
    public class CourierService : ICourierService
    {
        private readonly IUnitofWork _unitOfWork;
        private readonly IMapServices _mapServices;
        private readonly IHubContext<NegotiationHub> _hubContext;
        private readonly ILogger<CourierService> _logger;

        public CourierService(IUnitofWork unitofWork, IMapServices mapServices, 
            IHubContext<NegotiationHub> hubContext, ILogger<CourierService> logger)
        {
            _unitOfWork = unitofWork;
            _mapServices = mapServices;
            _hubContext = hubContext;
            _logger = logger;
        }

        private double CalculateFare(decimal price, double distanceMeters)
        {
            distanceMeters /= 1000;
            double fare = (double)price * distanceMeters;
            return fare;
        }

        public async Task<NotificationResult> NotifyServiceProviders(CourierServiceDto booking)
        {
            var response = new NotificationResult();
            try
            {
                var providers = await _unitOfWork.RiderorPark.GetAllByExpression(x => x.ServiceProviderRole == ServiceProviderRole.Freelancer && x.IsAvailable == true);
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
                        var distdura = await _mapServices.GetDistanceAndDurationAsync(provider.AddressorLocation, booking.PickupLocation);
                        var result = await _mapServices.GetDistanceAsync(provider.AddressorLocation, booking.PickupLocation);
                        var element = result.rows.FirstOrDefault()?.elements.FirstOrDefault();
                        if (element == null || element.status != "OK")
                        {
                            _logger.LogWarning($"Failed to calculate distance");
                            continue;
                        }

                        if (element != null && element.status == "OK" && distdura.DistanceMeters <= 5000)
                        {

                            booking.DistanceToPickupLocation = element.distance.text;
                            booking.DurationToPickupLocation = element.duration.text;


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
                _logger.LogError(ex, "Error in NotifyServiceProvidersAsync for booking ID {BookingId}", booking.CourierServiceId);
            }

            return response;
        }


        public async Task<Response<CourierServiceDto>> Book(Guid courierServiceId)
        {
            var response = new Response<CourierServiceDto>();
            var booking = await _unitOfWork.Courier.GetAsync(x => x.Id == courierServiceId);
            if (booking == null)
            {
                response.Message = "Booking not found";
                response.Success = false;
                return response;
            }
            if (booking.RiderorParkId != null)
            {
                booking.ServiceStatus = ServiceStatus.Booked;
                booking.TotalPrice = booking.EstimatedPrice;
                await _unitOfWork.Courier.UpdateAsync(booking);
                response.Message = "Booking Sent";
                response.Success = true;
                response.Data = new CourierServiceDto
                {
                    CourierServiceId = booking.Id,
                    CustomerId = booking.CustomerId,
                    RiderorParkId = booking.RiderorParkId,
                    PickupLocation = booking.PickupLocation,
                    Destination = booking.Destination,
                    Distance = booking.Distance,
                    Duration = booking.Duration,
                    DistanceToPickupLocation = booking.DistanceToPickupLocation,
                    DurationToPickupLocation = booking.DurationToPickupLocation,
                    TotalPrice = booking.TotalPrice,
                    ServiceStatus = booking.ServiceStatus.ToString()
                };
                return response;
            }
            booking.ServiceStatus = ServiceStatus.Pending;
            await _unitOfWork.Courier.UpdateAsync(booking);
            response.Message = "Booking Sent, waiting for rider to accept";
            response.Success = true;
            var data = new CourierServiceDto
            {
                CourierServiceId = booking.Id,
                CustomerId = booking.CustomerId,
                RiderorParkId = booking.RiderorParkId,
                PickupLocation = booking.PickupLocation,
                Destination = booking.Destination,
                Distance = booking.Distance,
                Duration = booking.Duration,
                EstimatedPrice = booking.EstimatedPrice,
                ServiceStatus = booking.ServiceStatus.ToString()
            };
            var not = await NotifyServiceProviders(data);
            data.ProviderNotified = not.ProvidersNotified;
            response.Data = data;
            return response;
        }

        public async Task<Response<bool>> CancelBooking(Guid courierServiceId)
        {
            var response = new Response<bool>();
            var booking = await _unitOfWork.Courier.GetAsync(x => x.Id == courierServiceId);
            if (booking == null)
            {
                response.Message = "Booking not found";
                response.Success = false;
                return response;
            }
            booking.ServiceStatus = ServiceStatus.Cancelled;
            await _unitOfWork.Courier.UpdateAsync(booking);
            response.Message = "Booking Cancelled";
            response.Success = true;
            response.Data = true;
            return response;
        }

        public async Task<Response<IEnumerable<CourierServiceDto>>> GetBookings(Guid riderorParkUserIdORcustomerId)
        {
            var response = new Response<IEnumerable<CourierServiceDto>>();
            var bookings = await _unitOfWork.Courier.Gets(x => x.RiderorPark.UserId == riderorParkUserIdORcustomerId 
            || x.CustomerId == riderorParkUserIdORcustomerId && x.ServiceStatus != ServiceStatus.Init);

            if (bookings == null)
            {
                response.Success = false;
                response.Message = "Booking Not found";
                return response;
            }

            response.Data = bookings.Select(booking => new CourierServiceDto
            {
                CourierServiceId = booking.Id,
                CustomerId = booking.CustomerId,
                RiderorParkId = booking.RiderorParkId,
                PickupLocation = booking.PickupLocation,
                Destination = booking.Destination,
                Distance = booking.Distance,
                Duration = booking.Duration,
                DistanceToPickupLocation = booking.DistanceToPickupLocation,
                DurationToPickupLocation = booking.DurationToPickupLocation,
                EstimatedPrice = booking.EstimatedPrice,
                TotalPrice = booking.TotalPrice,
                ServiceStatus = booking.ServiceStatus.ToString()
            });
            response.Success = true;
            response.Message = "Bookings gotten";
            return response;
        }

        public async Task<Response<IEnumerable<CourierServiceDto>>> GetPendings(string location)
        {
            var response = new Response<IEnumerable<CourierServiceDto>>();
            var bookings = await _unitOfWork.Courier.GetAllByExpression(x => x.ServiceStatus == ServiceStatus.Pending);
            if (bookings == null)
            {
                response.Success = false;
                response.Message = "Booking Not found";
                return response;
            }

            var bookingsDto = new List<CourierServiceDto>();

            foreach (var booking in bookings)
            {
                var distandDura = await _mapServices.GetDistanceAndDurationAsync(location, booking.PickupLocation);

                var result = await _mapServices.GetDistanceAsync(location, booking.PickupLocation);
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
                    bookingsDto.Add(new CourierServiceDto
                    {
                        CourierServiceId = booking.Id,
                        CustomerId = booking.CustomerId,
                        RiderorParkId = booking.RiderorParkId,
                        PickupLocation = booking.PickupLocation,
                        Destination = booking.Destination,
                        Distance = booking.Distance,
                        Duration = booking.Duration,
                        DistanceToPickupLocation = distance,
                        DurationToPickupLocation = duration,
                        EstimatedPrice = booking.EstimatedPrice,
                        TotalPrice = booking.TotalPrice,
                        ServiceStatus = booking.ServiceStatus.ToString()
                    });
                }
            }
            response.Message = "Pending Bookings gotten";
            response.Success = true;
            response.Data = bookingsDto;
            return response;
        }

        public async Task<Response<CourierServiceDto>> InitBooking(Guid customerId, CourierServiceRequest request, Guid? riderorparkUserId)
        {
            var response = new Response<CourierServiceDto>();
            RiderorPark park = null;
            if (riderorparkUserId != null)
            {
                park = await _unitOfWork.RiderorPark.GetAsync(x => x.UserId == riderorparkUserId);
                if (park == null)
                {
                    response.Message = "Park not found";
                    response.Success = false;
                    return response;
                }
            }

            var result = await _mapServices.GetDistanceAsync(request.PickupLocation, request.Destination);
            var element = result.rows.FirstOrDefault()?.elements.FirstOrDefault();
            if (element == null || element.status != "OK")
            {
                response.Message = "Failed to calculate distance and duration.";
                response.Success = false;
                return response;
            }

            var distance = element.distance.text;
            var duration = element.duration.text;
            var (distanceMeters, durationSeconds) = await _mapServices.GetDistanceAndDurationAsync(request.PickupLocation, request.Destination);

            var vehicle = await _unitOfWork.Context.CourierVehicles.FirstOrDefaultAsync(x => x.Id == request.VehicleId);
            if (vehicle == null)
            {
                response.Message = "Vehicle Not found";
                response.Success = false;
                return response;
            }

            Courier_Service booking = new Courier_Service();
            if (park != null)
            {

                var res = await _mapServices.GetDistanceAsync(park.AddressorLocation, request.PickupLocation);
                var elem = res.rows.FirstOrDefault()?.elements.FirstOrDefault();
                var distanceToPickupLocation = elem?.distance.text;
                var durationToPickupLocation = elem?.duration.text;

                booking.CustomerId = customerId;
                booking.RiderorParkId = park.Id;
                booking.PickupLocation = request.PickupLocation;
                booking.Destination = request.Destination;
                booking.VehicleId = vehicle.Id;
                booking.Distance = distance;
                booking.Duration = duration;
                booking.DistanceToPickupLocation = distanceToPickupLocation;
                booking.DurationToPickupLocation = durationToPickupLocation;
                booking.EstimatedPrice = (decimal)CalculateFare(vehicle.Price, distanceMeters);
            }
            else
            {

                booking.CustomerId = customerId;
                booking.PickupLocation = request.PickupLocation;
                booking.Destination = request.Destination;
                booking.VehicleId = vehicle.Id;
                booking.Distance = distance;
                booking.Duration = duration;
                booking.EstimatedPrice = (decimal)CalculateFare(vehicle.Price, distanceMeters);
            }

            booking.ServiceStatus = ServiceStatus.Init;
            await _unitOfWork.Courier.AddAsync(booking);

            response.Data = new CourierServiceDto
            {
                CourierServiceId = booking.Id,
                CustomerId = booking.CustomerId,
                RiderorParkId = booking.RiderorParkId,
                Vehicle = vehicle.VehicleType,
                PickupLocation = booking.PickupLocation,
                Destination = booking.Destination,
                Distance = booking.Distance,
                Duration = booking.Duration,
                DistanceToPickupLocation = booking.DistanceToPickupLocation,
                DurationToPickupLocation = booking.DurationToPickupLocation,
                EstimatedPrice = booking.EstimatedPrice,
                TotalPrice = booking.TotalPrice,
                ServiceStatus = booking.ServiceStatus.ToString()
            };
            response.Success = true;
            response.Message = "Initiated";
            return response;
        }

        public async Task<Response<bool>> AcceptBooking(Guid courierServiceId)
        {
            var response = new Response<bool>();
            var booking = await _unitOfWork.Courier.GetAsync(x => x.Id == courierServiceId);
            if (booking == null)
            {
                response.Message = "Booking not found";
                response.Success = false;
                return response;
            }
            booking.ServiceStatus = ServiceStatus.InProgress;
            await _unitOfWork.Courier.UpdateAsync(booking);
            response.Message = "Booking Booked";
            response.Success = true;
            response.Data = true;
            return response;
        }

        public async Task<Response<CourierServiceDto>> AcceptOffer(Guid bookingId, Guid riderUserId, decimal offerAmount)
        {
            var response = new Response<CourierServiceDto>();
            var booking = await _unitOfWork.Courier.GetAsync(x => x.Id == bookingId);
            if (booking == null)
            {
                response.Message = "Booking not found";
                response.Success = false;
                return response;
            }
            var rider = await _unitOfWork.RiderorPark.GetAsync(x => x.UserId == riderUserId);
            if (rider == null)
            {
                response.Message = "Rider not found";
                response.Success = false;
                return response;
            }
            booking.TotalPrice = offerAmount;
            booking.ServiceStatus = ServiceStatus.Booked;
            await _unitOfWork.Courier.UpdateAsync(booking);
            response.Message = "Offer accepted successfully";
            response.Success = true;


            var res = await _mapServices.GetDistanceAsync(rider.AddressorLocation, booking.PickupLocation);
            var elem = res.rows.FirstOrDefault()?.elements.FirstOrDefault();
            var distanceToPickupLocation = elem?.distance.text;
            var durationToPickupLocation = elem?.duration.text;
            if (distanceToPickupLocation == null || durationToPickupLocation == null)
            {
                response.Message = "Failed to calculate distance or duration.";
                response.Success = false;
                return response;
            }
            var v = await _unitOfWork.Context.CourierVehicles.FirstOrDefaultAsync(x => x.Id == booking.VehicleId);
            response.Data = new CourierServiceDto
            {
                CourierServiceId = booking.Id,
                CustomerId = booking.CustomerId,
                RiderorParkId = booking.RiderorParkId,
                Vehicle = v.VehicleType,
                PickupLocation = booking.PickupLocation,
                Destination = booking.Destination,
                Distance = booking.Distance,
                Duration = booking.Duration,
                DistanceToPickupLocation = distanceToPickupLocation,
                DurationToPickupLocation = durationToPickupLocation,
                EstimatedPrice = booking.EstimatedPrice,
                TotalPrice = booking.TotalPrice,
                ServiceStatus = booking.ServiceStatus.ToString()
            };
            response.Success = true;
            response.Message = "Offer accepted successfully";
            return response;
        }

        public async Task<Response<CourierServiceDto>> RaisePrice(Guid bookingId, decimal newPrice)
        {
            var response = new Response<CourierServiceDto>();
            var booking = await _unitOfWork.Courier.GetAsync(x => x.Id == bookingId);
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
            await _unitOfWork.Courier.UpdateAsync(booking);
            response.Message = "Price raised successfully";
            response.Success = true;
            var data = new CourierServiceDto
            {
                CourierServiceId = booking.Id,
                CustomerId = booking.CustomerId,
                RiderorParkId = booking.RiderorParkId,
                PickupLocation = booking.PickupLocation,
                Destination = booking.Destination,
                Distance = booking.Distance,
                Duration = booking.Duration,
                DistanceToPickupLocation = booking.DistanceToPickupLocation,
                DurationToPickupLocation = booking.DurationToPickupLocation,
                EstimatedPrice = booking.EstimatedPrice,
                TotalPrice = booking.TotalPrice,
                ServiceStatus = booking.ServiceStatus.ToString()
            };
            var notify = await NotifyServiceProviders(response.Data);
            data.ProviderNotified = notify.ProvidersNotified;
            response.Data = data;
            return response;
        }
    }
}
