using CaptainTrackBackend.Application.Abstraction.Interface.Maps;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.House_Moving;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.Ambulance;
using CaptainTrackBackend.Application.DTO.ServiceProviders.CarWash;
using CaptainTrackBackend.Application.DTO.ServiceProviders.House_Moving;
using CaptainTrackBackend.Domain.Entities;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.Ambulance;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.House_Moving;
using CaptainTrackBackend.Domain.Enum;
using CaptainTrackBackend.Domain.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Services.ServiceProviders.House_Moving
{
    public class HouseMovingService : IHouseMovingService
    {
        private readonly IUnitofWork _unitOfWork;
        private readonly IMapServices _mapServices;
        private readonly IHubContext<NegotiationHub> _hubContext;
        private readonly ILogger<HouseMovingService> _logger;
        public HouseMovingService( IUnitofWork unitofWork, IMapServices mapServices, 
            IHubContext<NegotiationHub> hubContext, ILogger<HouseMovingService> logger) 
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

        public async Task<NotificationResult> NotifyServiceProviders(HouseMovingDto booking)
        {
            var response = new NotificationResult();
            try
            {
                var providers = await _unitOfWork.HouseMover.GetAllByExpression(x => x.ServiceProviderRole == ServiceProviderRole.Freelancer && x.IsAvailable == true);
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

                            booking.DistanceToPickup = element.distance.text;
                            booking.DurationToPickup = element.duration.text;


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
                _logger.LogError(ex, "Error in NotifyServiceProvidersAsync for booking ID {BookingId}", booking.HouseMoverId);
            }
            return response;
        }



        public async Task<Response<HouseMovingDto>> InitiateBooking(Guid customerId, HouseMovingRequest request, Guid? houseMoverUserId = null)
        {
            var response = new Response<HouseMovingDto>();
            HouseMover houseMover = null;
            if (houseMoverUserId != null)
            {
                houseMover = await _unitOfWork.HouseMover.GetAsync(x => x.UserId == houseMoverUserId);
                if (houseMover == null)
                {
                    response.Message = "HouseMover not found";
                    response.Success = false;
                    return response;
                }
            }
            var result = await _mapServices.GetDistanceAsync(request.PickupLocation, request.DropoffLocation);
            var element = result.rows.FirstOrDefault()?.elements.FirstOrDefault();
            if (element == null || element.status != "OK")
            {
                response.Message = "Failed to calculate distance and duration.";
                response.Success = false;
                return response;
            }
            var distance = element.distance.text;
            var duration = element.duration.text;
            var (distanceMeters, durationSeconds) = await _mapServices.GetDistanceAndDurationAsync(request.PickupLocation, request.DropoffLocation);

            var truck = await _unitOfWork.Context.HouseMovingTrucks.FirstOrDefaultAsync(x => x.Id == request.TruckId);
            if (truck == null) { response.Message = "Truck not found"; response.Success = false; return response; }

            HouseMoving booking = new HouseMoving();
            if (houseMover != null)
            {
                var res = await _mapServices.GetDistanceAsync(houseMover.AddressorLocation, request.PickupLocation);
                var elem = res.rows.FirstOrDefault()?.elements.FirstOrDefault();
                var distanceToPickupLocation = elem?.distance.text;
                var durationToPickupLocation = elem?.duration.text;

                booking.CustomerId = customerId;
                booking.HouseMoverId = houseMover.Id;
                booking.PickupLocation = request.PickupLocation;
                booking.DropoffLocation = request.DropoffLocation;
                booking.Distance = distance;
                booking.Duration = duration;
                booking.DistanceToPickup = distanceToPickupLocation;
                booking.DurationToPickup = durationToPickupLocation;
                booking.PackageId = request.PackageId;
                booking.TruckId = truck.Id;
                booking.EstimatedFare = (decimal)CalculateFare(truck.Price, distanceMeters);
            }
            else
            {
                booking.CustomerId = customerId;
                booking.PickupLocation = request.PickupLocation;
                booking.DropoffLocation = request.DropoffLocation;
                booking.Distance = distance;
                booking.Duration = duration;
                booking.PackageId = request.PackageId;
                booking.TruckId = truck.Id;
                booking.EstimatedFare = (decimal)CalculateFare(truck.Price, distanceMeters);
            }

            booking.Status = ServiceStatus.Init;
            await _unitOfWork.HouseMoving.AddAsync(booking);

            response.Message = "Booking Initiated succecfully";
            response.Success = true;
            response.Data = new HouseMovingDto
            {
                HouseMovingId = booking.Id,
                CustomerId = booking.CustomerId,
                HouseMoverId = booking.HouseMoverId,
                PickupLocation = booking.PickupLocation,
                DropoffLocation = booking.DropoffLocation,
                Distance = booking.Distance,
                Duration = booking.Duration,
                DistanceToPickup = booking.DistanceToPickup,
                DurationToPickup = booking.DurationToPickup,
                EstimatedFare = booking.EstimatedFare,
                Status = booking.Status.ToString()
            };
            return response;
        }

        public async Task<Response<HouseMovingDto>> Book(Guid houseMovingId)
        {
            var response = new Response<HouseMovingDto>();
            var houseMoving = await _unitOfWork.HouseMoving.GetAsync(x => x.Id == houseMovingId);
            if (houseMoving == null)
            {
                response.Message = "Not Found";
                response.Success = false;
                return response;
            }
            if (houseMoving.HouseMoverId != null)
            {
                houseMoving.Status = ServiceStatus.Booked;
                houseMoving.Price = houseMoving.EstimatedFare;
                await _unitOfWork.HouseMoving.UpdateAsync(houseMoving);
                response.Message = "Booking Sent";
                response.Success = true;
                response.Data = new HouseMovingDto
                {
                    HouseMovingId = houseMoving.Id,
                    CustomerId = houseMoving.CustomerId,
                    HouseMoverId = houseMoving.HouseMoverId,
                    PickupLocation = houseMoving.PickupLocation,
                    DropoffLocation = houseMoving.DropoffLocation,
                    Distance = houseMoving.Distance,
                    Duration = houseMoving.Duration,
                    DistanceToPickup = houseMoving.DistanceToPickup,
                    DurationToPickup = houseMoving.DurationToPickup,
                    Price = houseMoving.Price,
                    Status = houseMoving.Status.ToString()
                };
                return response;
            }
            else
            {
                houseMoving.Status = ServiceStatus.Pending;
                await _unitOfWork.HouseMoving.UpdateAsync(houseMoving);
                response.Message = "Booking Sent, awaiting service provider to accept";
                response.Success = true;
                var date = new HouseMovingDto
                {
                    HouseMovingId = houseMoving.Id,
                    CustomerId = houseMoving.CustomerId,
                    HouseMoverId = houseMoving.HouseMoverId,
                    PickupLocation = houseMoving.PickupLocation,
                    DropoffLocation = houseMoving.DropoffLocation,
                    Distance = houseMoving.Distance,
                    Duration = houseMoving.Duration,
                    EstimatedFare = houseMoving.EstimatedFare,
                    Price = houseMoving.Price,
                    Status = houseMoving.Status.ToString()
                };
                var notificationResult = await NotifyServiceProviders(date);
                date.ProvidersNotified = notificationResult.ProvidersNotified;
                response.Data = date;
                return response;
            }

        }

        public async Task<Response<IEnumerable<HouseMovingDto>>> GetBookings(Guid userId)
        {
            var response = new Response<IEnumerable<HouseMovingDto>>();
            var houseMovings = await _unitOfWork.HouseMoving.GetHouseMovingsAsync(x => x.HouseMover.UserId == userId || x.CustomerId == userId && x.Status != ServiceStatus.Init);
            if (houseMovings == null)
            {
                response.Message = "No houseMovings";
                response.Success = false;
                return response;
            }
            response.Message = "Pending Bookings gotten";
            response.Success = true;
            response.Data = houseMovings.Select(x => new HouseMovingDto
            {
                HouseMovingId = x.Id,
                CustomerId = x.CustomerId,
                HouseMoverId = x.HouseMoverId,
                PickupLocation = x.PickupLocation,
                DropoffLocation = x.DropoffLocation,
                Distance = x.Distance,
                Duration = x.Duration,
                DistanceToPickup = x.DistanceToPickup,
                DurationToPickup = x.DurationToPickup,
                EstimatedFare = x.EstimatedFare,
                Price = x.Price,
                Status = x.Status.ToString()
            });
            return response;

            throw new NotImplementedException();
        }

        public async Task<Response<IEnumerable<HouseMovingDto>>> GetPending(string location)
        {
            var response = new Response<IEnumerable<HouseMovingDto>>();
            var houseMovings = await _unitOfWork.HouseMoving.GetAllByExpression(x => x.Status == ServiceStatus.Pending);
            if (houseMovings == null)
            {
                response.Message = "No pending houseMovings";
                response.Success = false;
                return response;
            }

            var bookings = new List<HouseMovingDto>();

            foreach (var x in houseMovings)
            {
                var distandDura = await _mapServices.GetDistanceAndDurationAsync(location, x.PickupLocation);

                var result = await _mapServices.GetDistanceAsync(location, x.PickupLocation);
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
                    bookings.Add(new HouseMovingDto
                    {
                        HouseMovingId = x.Id,
                        CustomerId = x.CustomerId,
                        HouseMoverId = x.HouseMoverId,
                        PickupLocation = x.PickupLocation,
                        DropoffLocation = x.DropoffLocation,
                        Distance = x.Distance,
                        Duration = x.Duration,
                        DistanceToPickup = distance,
                        DurationToPickup = duration,
                        EstimatedFare = x.EstimatedFare,
                        Price = x.Price,
                        Status = x.Status.ToString()
                    });
                }

            }

            response.Message = "Pending Bookings gotten";
            response.Success = true;
            response.Data = houseMovings.Select(x => new HouseMovingDto
            {
                HouseMovingId = x.Id,
                CustomerId = x.CustomerId,
                HouseMoverId = x.HouseMoverId,
                PickupLocation = x.PickupLocation,
                DropoffLocation = x.DropoffLocation,
                Distance = x.Distance,
                Duration = x.Duration,
                DistanceToPickup = x.DistanceToPickup,
                DurationToPickup = x.DurationToPickup,
                EstimatedFare = x.EstimatedFare,
                Price = x.Price,
                Status = x.Status.ToString()
            });
            return response;
        }

        public async Task<Response<bool>> AcceptBooking(Guid houseMovingId)
        {
            var response = new Response<bool>();
            var booking = await _unitOfWork.HouseMoving.GetAsync(x => x.Id == houseMovingId);
            if (booking == null)
            {
                response.Success = false;
                response.Message = "Booking not found.";
                return response;
            }
            booking.Status = ServiceStatus.Booked;
            await _unitOfWork.HouseMoving.UpdateAsync(booking);
            response.Success = true;
            response.Message = "Booking accepted successfully.";
            return response;
        }

        public async Task<Response<bool>> RejectBooking(Guid houseMovingId)
        {
            var response = new Response<bool>();
            var booking = await _unitOfWork.HouseMoving.GetAsync(x => x.Id == houseMovingId);
            if (booking == null)
            {
                response.Success = false;
                response.Message = "Booking not found.";
                return response;
            }
            booking.Status = ServiceStatus.Rejected;
            await _unitOfWork.HouseMoving.UpdateAsync(booking);
            response.Success = true;
            response.Message = "Booking Rejected successfully.";
            return response;
        }

        public async Task<Response<bool>> CancelBooking(Guid houseMovingId)
        {
            var response = new Response<bool>();
            var booking = await _unitOfWork.HouseMoving.GetAsync(x => x.Id == houseMovingId);
            if (booking == null)
            {
                response.Success = false;
                response.Message = "Booking not found.";
                return response;
            }
            booking.Status = ServiceStatus.Booked;
            await _unitOfWork.HouseMoving.UpdateAsync(booking);
            response.Success = true;
            response.Message = "Booking Canceled successfully.";
            return response;
        }

        public async Task<Response<HouseMovingDto>> AcceptOffer(Guid bookingId, Guid houseMoverUserId, decimal offerAmount)
        {
            var response = new Response<HouseMovingDto>();
            var booking = await _unitOfWork.HouseMoving.GetAsync(x => x.Id == bookingId);
            if (booking == null)
            {
                response.Message = "Booking not found";
                response.Success = false;
                return response;
            }
            var houseMover = await _unitOfWork.HouseMover.GetAsync(x => x.UserId == houseMoverUserId);
            booking.HouseMoverId = houseMover.Id;
            booking.Price = offerAmount;
            booking.Status = ServiceStatus.Booked;
            await _unitOfWork.HouseMoving.UpdateAsync(booking);
            response.Message = "Offer accepted successfully";
            response.Success = true;


            var res = await _mapServices.GetDistanceAsync(houseMover.AddressorLocation, booking.PickupLocation);
            var elem = res.rows.FirstOrDefault()?.elements.FirstOrDefault();
            var distanceToPickupLocation = elem?.distance.text;
            var durationToPickupLocation = elem?.duration.text;
            if (distanceToPickupLocation == null || durationToPickupLocation == null)
            {
                response.Message = "Failed to calculate distance or duration.";
                response.Success = false;
                return response;
            }

            response.Data = new HouseMovingDto
            {
                HouseMovingId = booking.Id,
                CustomerId = booking.CustomerId,
                HouseMoverId = booking.HouseMoverId,
                PickupLocation = booking.PickupLocation,
                DropoffLocation = booking.DropoffLocation,
                Distance = booking.Distance,
                Duration = booking.Duration,
                DistanceToPickup = distanceToPickupLocation,
                DurationToPickup = durationToPickupLocation,
                EstimatedFare = booking.EstimatedFare,
                Price = booking.Price,
                Status = booking.Status.ToString()
            };
            response.Success = true;
            response.Message = "Offer accepted successfully";
            return response;
        }

        public async Task<Response<HouseMovingDto>> RaisePrice(Guid bookingId, decimal newPrice)
        {
            var response = new Response<HouseMovingDto>();
            var booking = await _unitOfWork.HouseMoving.GetAsync(x => x.Id == bookingId);
            if (booking == null)
            {
                response.Message = "Booking not found";
                response.Success = false;
                return response;
            }
            if (newPrice <= booking.EstimatedFare)
            {
                response.Success = false;
                response.Message = "You can only increase the price.";
                return response;
            }
            booking.EstimatedFare = newPrice;
            await _unitOfWork.HouseMoving.UpdateAsync(booking);
            response.Message = "Price raised successfully";
            response.Success = true;
            var data = new HouseMovingDto
            {
                HouseMovingId = booking.Id,
                CustomerId = booking.CustomerId,
                HouseMoverId = booking.HouseMoverId,
                PickupLocation = booking.PickupLocation,
                DropoffLocation = booking.DropoffLocation,
                Distance = booking.Distance,
                Duration = booking.Duration,
                DistanceToPickup = booking.DistanceToPickup,
                DurationToPickup = booking.DurationToPickup,
                EstimatedFare = booking.EstimatedFare,
                Price = booking.Price,
                Status = booking.Status.ToString()
            };
            var notify = await NotifyServiceProviders(response.Data);
            data.ProvidersNotified = notify.ProvidersNotified;
            response.Data = data;
            return response;
        }
    }
}
