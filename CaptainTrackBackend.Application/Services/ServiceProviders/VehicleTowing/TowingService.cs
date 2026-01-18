using CaptainTrackBackend.Application.Abstraction.Interface.Maps;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.VehicleTowing;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.CarWash;
using CaptainTrackBackend.Application.DTO.ServiceProviders.Ridehailing;
using CaptainTrackBackend.Application.DTO.ServiceProviders.VehicleTowing;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.RideHailing;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.VehicleTowing;
using CaptainTrackBackend.Domain.Enum;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Services.ServiceProviders.VehicleTowing
{
    public class TowingService : ITowingService
    {
        private readonly IUnitofWork _unitOfWork;
        private readonly IMapServices _mapServices;
        private readonly IHubContext<NegotiationHub> _hubContext;
        private readonly ILogger<TowingService> _logger;


        public TowingService(IUnitofWork unitofWork, IMapServices mapServices, IHubContext<NegotiationHub> hubContext, ILogger<TowingService> logger)
        {
            _unitOfWork = unitofWork;
            _mapServices = mapServices;
            _hubContext = hubContext;
            _logger = logger;
        }


        private double CalculateFare(decimal price, double distanceMeters, double durationSeconds)
        {
            distanceMeters /= 1000;
            durationSeconds /= 60;
            double fare = (double)price * distanceMeters + (double)price * durationSeconds;
            return fare;
        }


        public async Task<NotificationResult> NotifyServiceProviders(TowingDto booking)
        {
            var response = new NotificationResult();
            try
            {
                var providers = await _unitOfWork.TruckOperator.GetAllByExpression(x => x.ServiceProviderRole == ServiceProviderRole.Freelancer && x.IsAvailable == true);
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

                            booking.DistanceBtwnTruckAndPickup = element.distance.text;
                            booking.DurationBtwnTruckAndPickup = element.duration.text;


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
                _logger.LogError(ex, "Error in NotifyServiceProvidersAsync for booking ID {BookingId}", booking.TowTruckOperatorId);
            }

            return response;
        }



        public async Task<Response<TowingDto>> Book(Guid towingId)
        {
            var response = new Response<TowingDto>();
            var towing = await _unitOfWork.Towing.GetAsync(x => x.Id == towingId);
            if (towing == null)
            {
                response.Message = "Towing not found";
                response.Success = false;
                return response;
            }
            if (towing.TowTruckOperatorId != null)
            {
                towing.ServiceStatus = ServiceStatus.Booked;
                towing.TotalPrice = towing.EstimatedPrice;
                await _unitOfWork.Towing.UpdateAsync(towing);
                response.Message = "Booking Sent";
                response.Success = true;
                response.Data = new TowingDto
                {
                    TowingId = towing.Id,
                    CustomerId = towing.CustomerId,
                    TowTruckOperatorId = towing.TowTruckOperatorId,
                    TruckId = towing.TruckId,
                    TruckName = towing.TruckName,
                    PickupLocation = towing.PickupLocation,
                    DropOffLocation = towing.DropOffLocation,
                    CarModel = towing.CarModel,
                    Distance = towing.Distance,
                    Duration = towing.Duration,
                    DistanceBtwnTruckAndPickup = towing.DistanceBtwnTruckAndPickup,
                    DurationBtwnTruckAndPickup = towing.DurationBtwnTruckAndPickup,
                    TotalPrice = towing.TotalPrice
                };
                return response;
            }
            else
            {
                towing.ServiceStatus = ServiceStatus.Pending;
                await _unitOfWork.Towing.UpdateAsync(towing);
                response.Message = "Booking Sent, waiting for truck operator to accept";
                response.Success = true;
                var data = new TowingDto
                {
                    TowingId = towing.Id,
                    CustomerId = towing.CustomerId,
                    TruckId = towing.TruckId,
                    TruckName = towing.TruckName,
                    PickupLocation = towing.PickupLocation,
                    DropOffLocation = towing.DropOffLocation,
                    CarModel = towing.CarModel,
                    Distance = towing.Distance,
                    Duration = towing.Duration,
                    EstimatedPrice = towing.EstimatedPrice
                };
                var notificationResult = await NotifyServiceProviders(data);
                data.ProvidersNotified = notificationResult.ProvidersNotified;
                response.Data = data;
                return response;
            }
        }

        public async Task<Response<TowingDto>> InitBooking(Guid customerId, TowingRequest request, Guid? truckOperatorUserId = null)
        {
            var response = new Response<TowingDto>();
            TowTruckOperator truckOperator = null;
            if (truckOperatorUserId != null)
            {
                truckOperator = await _unitOfWork.TruckOperator.GetAsync(x => x.UserId == truckOperatorUserId);
                if (truckOperator == null)
                {
                    response.Message = "Truck operator not found";
                    response.Success = false;
                    return response;
                }
            }
            var result = await _mapServices.GetDistanceAsync(request.PickupLocation, request.DropOffLocation);
            var element = result.rows.FirstOrDefault()?.elements.FirstOrDefault();
            if (element == null || element.status != "OK")
            {
                response.Message = "Failed to calculate distance and duration.";
                response.Success = false;
                return response;
            }
            var distance = element.distance.text;
            var duration = element.duration.text;
            var truck = await _unitOfWork.Context.Trucks.FirstOrDefaultAsync(x => x.Id == request.TruckId);
            Towing towing = new Towing();
            if (truckOperator != null)
            {
                var res = await _mapServices.GetDistanceAsync(truckOperator.AddressorLocation, request.PickupLocation);
                var elem = res.rows.FirstOrDefault()?.elements.FirstOrDefault();
                var distanceBtwnTruckAndPickup = elem?.distance.text;
                var durationBtwnTruckAndPickup = elem?.duration.text;

                towing.CustomerId = customerId;
                towing.TowTruckOperatorId = truckOperator.Id;
                towing.TruckId = truck.Id;
                towing.TruckName = truck.Name;
                towing.PickupLocation = request.PickupLocation;
                towing.DropOffLocation = request.DropOffLocation;
                towing.CarModel = request.CarModel;
                towing.Distance = distance;
                towing.Duration = duration;
                towing.DistanceBtwnTruckAndPickup = distanceBtwnTruckAndPickup;
                towing.DurationBtwnTruckAndPickup = durationBtwnTruckAndPickup;
            }
            else
            {
                towing.CustomerId = customerId;
                towing.TruckId = truck.Id;
                towing.TruckName = truck.Name;
                towing.PickupLocation = request.PickupLocation;
                towing.DropOffLocation = request.DropOffLocation;
                towing.CarModel = request.CarModel;
                towing.Distance = distance;
                towing.Duration = duration;
            }

            var (distanceMeters, durationSeconds) = await _mapServices.GetDistanceAndDurationAsync(request.PickupLocation, request.DropOffLocation);
            var estimatedFare = CalculateFare(truck.Amount, distanceMeters, durationSeconds);
            towing.EstimatedPrice = (decimal)estimatedFare;
            towing.ServiceStatus = ServiceStatus.Init;
            await _unitOfWork.Towing.AddAsync(towing);

            response.Message = "Booking Initiated succecfully";
            response.Success = true;
            response.Data = new TowingDto
            {
                TowingId = towing.Id,
                CustomerId = towing.CustomerId,
                TowTruckOperatorId = towing.TowTruckOperatorId,
                TruckId = towing.TruckId,
                TruckName = towing.TruckName,
                PickupLocation = towing.PickupLocation,
                DropOffLocation = towing.DropOffLocation,
                CarModel = towing.CarModel,
                Distance = towing.Distance,
                Duration = towing.Duration,
                DistanceBtwnTruckAndPickup = towing.DistanceBtwnTruckAndPickup,
                DurationBtwnTruckAndPickup = towing.DurationBtwnTruckAndPickup,
                EstimatedPrice = towing.EstimatedPrice
            };
            return response;
        }

        public async Task<Response<List<TowingDto>>> GetPendings(string location)
        {
            var response = new Response<List<TowingDto>>();
            var towings = await _unitOfWork.Towing.GetAllByExpression(t => t.ServiceStatus == ServiceStatus.Pending && t.TowTruckOperatorId == null);
            if (towings == null || !towings.Any())
            {
                response.Message = "No pending bookings found";
                response.Success = false;
                return response;
            }

            var towingDtos = new List<TowingDto>();

            foreach (var t in towings)
            {
                var distandDura = await _mapServices.GetDistanceAndDurationAsync(location, t.PickupLocation);

                var result = await _mapServices.GetDistanceAsync(location, t.PickupLocation);
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
                    towingDtos.Add(new TowingDto
                    {
                        TowingId = t.Id,
                        CustomerId = t.CustomerId,
                        TruckId = t.TruckId,
                        TruckName = t.TruckName,
                        PickupLocation = t.PickupLocation,
                        DropOffLocation = t.DropOffLocation,
                        CarModel = t.CarModel,
                        Distance = t.Distance,
                        Duration = t.Duration,
                        DistanceBtwnTruckAndPickup = distance,
                        DurationBtwnTruckAndPickup = duration,
                        EstimatedPrice = t.EstimatedPrice,
                        ServiceStatus = t.ServiceStatus.ToString()
                    });
                }

            }

            response.Message = "Pending bookings retrieved successfully";
            response.Success = true;
            response.Data = towingDtos;
            return response;
        }

        public async Task<Response<List<TowingDto>>> GetBookings(Guid userId)
        {
            var response = new Response<List<TowingDto>>();
            //var truckOperator = await _unitOfWork.TruckOperator.GetAsync(x => x.UserId == userId);
            var towings = await _unitOfWork.Towing.GetBookings(x => x.TowTruckOperator.UserId == userId || x.CustomerId == userId && x.ServiceStatus != ServiceStatus.Init);
            if (towings == null || !towings.Any())
            {
                response.Message = "No bookings found for this truck operator";
                response.Success = false;
                return response;
            }

            var towingDtos = towings.Select(t => new TowingDto
            {
                TowingId = t.Id,
                CustomerId = t.CustomerId,
                TruckId = t.TruckId,
                TruckName = t.TruckName,
                PickupLocation = t.PickupLocation,
                DropOffLocation = t.DropOffLocation,
                CarModel = t.CarModel,
                Distance = t.Distance,
                Duration = t.Duration,
                DistanceBtwnTruckAndPickup = t.DistanceBtwnTruckAndPickup,
                DurationBtwnTruckAndPickup = t.DurationBtwnTruckAndPickup,
                EstimatedPrice = t.EstimatedPrice,
                ServiceStatus = t.ServiceStatus.ToString(),
                TotalPrice = t.TotalPrice,
            }).ToList();
            response.Message = "Bookings retrieved successfully";
            response.Success = true;
            response.Data = towingDtos;
            return response;
        }


        public async Task<Response<bool>> AcceptBooking(Guid towingId)
        {
            var response = new Response<bool>();
            var booking = await _unitOfWork.Towing.GetAsync(x => x.Id == towingId);
            if (booking == null)
            {
                response.Success = false;
                response.Message = "Booking not found.";
                return response;
            }
            booking.ServiceStatus = ServiceStatus.Booked;
            await _unitOfWork.Towing.UpdateAsync(booking);
            response.Success = true;
            response.Message = "Booking Booked successfully.";
            return response;

        }

        public async Task<Response<bool>> RejectBooking(Guid towingId)
        {
            var response = new Response<bool>();
            var booking = await _unitOfWork.Towing.GetAsync(x => x.Id == towingId);
            if (booking == null)
            {
                response.Success = false;
                response.Message = "Booking not found.";
                return response;
            }
            booking.ServiceStatus = ServiceStatus.Rejected;
            await _unitOfWork.Towing.UpdateAsync(booking);
            response.Success = true;
            response.Message = "Booking Rejected successfully.";
            return response;
            throw new NotImplementedException();
        }

        public async Task<Response<bool>> CancelBooking(Guid towingId)
        {
            var response = new Response<bool>();
            var booking = await _unitOfWork.Towing.GetAsync(x => x.Id == towingId);
            if (booking == null)
            {
                response.Success = false;
                response.Message = "Booking not found.";
                return response;
            }
            booking.ServiceStatus = ServiceStatus.Cancelled;
            await _unitOfWork.Towing.UpdateAsync(booking);
            response.Success = true;
            response.Message = "Booking cancelled successfully.";
            return response;
        }

        public async Task<Response<TowingDto>> AcceptOffer(Guid bookingId, Guid truckOperatorUserId, decimal offerAmount)
        {
            var response = new Response<TowingDto>();
            var booking = await _unitOfWork.Towing.GetAsync(x => x.Id == bookingId);
            if (booking == null)
            {
                response.Message = "Booking not found";
                response.Success = false;
                return response;
            }

            var towTruck = await _unitOfWork.TruckOperator.GetAsync(x => x.UserId == truckOperatorUserId);

            booking.TowTruckOperatorId = towTruck.Id;
            booking.TotalPrice = offerAmount;
            booking.ServiceStatus = ServiceStatus.Booked;
            await _unitOfWork.Towing.UpdateAsync(booking);
            response.Message = "Offer accepted successfully";
            response.Success = true;


            var res = await _mapServices.GetDistanceAsync(towTruck.AddressorLocation, booking.PickupLocation);
            var elem = res.rows.FirstOrDefault()?.elements.FirstOrDefault();
            var distanceToPickupLocation = elem?.distance.text;
            var durationToPickupLocation = elem?.duration.text;
            if (distanceToPickupLocation == null || durationToPickupLocation == null)
            {
                response.Message = "Failed to calculate distance or duration.";
                response.Success = false;
                return response;
            }

            response.Data = new TowingDto
            {
                TowingId = booking.Id,
                CustomerId = booking.CustomerId,
                TowTruckOperatorId = booking.TowTruckOperatorId,
                TruckId = booking.TruckId,
                TruckName = booking.TruckName,
                PickupLocation = booking.PickupLocation,
                DropOffLocation = booking.DropOffLocation,
                CarModel = booking.CarModel,
                Distance = booking.Distance,
                Duration = booking.Duration,
                DistanceBtwnTruckAndPickup = distanceToPickupLocation,
                DurationBtwnTruckAndPickup = durationToPickupLocation,
                EstimatedPrice = booking.EstimatedPrice,
                TotalPrice = booking.TotalPrice
            };
            response.Success = true;
            response.Message = "Offer accepted successfully";
            return response;

        }

        public async Task<Response<TowingDto>> RaisePrice(Guid bookingId, decimal newPrice)
        {
            var response = new Response<TowingDto>();
            var booking = await _unitOfWork.Towing.GetAsync(x => x.Id == bookingId);
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
            await _unitOfWork.Towing.UpdateAsync(booking);
            response.Message = "Price raised successfully";
            response.Success = true;
            var data = new TowingDto
            {
                TowingId = booking.Id,
                CustomerId = booking.CustomerId,
                TowTruckOperatorId = booking.TowTruckOperatorId,
                TruckId = booking.TruckId,
                TruckName = booking.TruckName,
                PickupLocation = booking.PickupLocation,
                DropOffLocation = booking.DropOffLocation,
                CarModel = booking.CarModel,
                Distance = booking.Distance,
                Duration = booking.Duration,
                DistanceBtwnTruckAndPickup = booking.DistanceBtwnTruckAndPickup,
                DurationBtwnTruckAndPickup = booking.DurationBtwnTruckAndPickup,
                EstimatedPrice = booking.EstimatedPrice
            };
            var notify = await NotifyServiceProviders(response.Data);
            data.ProvidersNotified = notify.ProvidersNotified;
            response.Data = data;
            return response;
        }
    }
}
