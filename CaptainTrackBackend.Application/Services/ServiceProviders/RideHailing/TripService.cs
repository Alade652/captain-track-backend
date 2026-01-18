using CaptainTrackBackend.Application.Abstraction.Interface.Maps;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.RideHailing;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.MapModels;
using CaptainTrackBackend.Application.DTO.ServiceProviders.CarWash;
using CaptainTrackBackend.Application.DTO.ServiceProviders.Ridehailing;
using CaptainTrackBackend.Domain.Entities;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.RideHailing;
using CaptainTrackBackend.Domain.Enum;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CaptainTrackBackend.Application.Services.ServiceProviders.RideHailing
{
    public class TripService : ITripService
    {
        private readonly IUnitofWork _unitOfWork;
        private readonly IMapServices _mapServices;
        private readonly IMemoryCache _cache;
        private readonly IHubContext<NegotiationHub> _hubContext;
        private readonly ILogger<TripService> _logger;

        public TripService(IUnitofWork unitofWork, IMapServices mapServices, IMemoryCache cache, 
            IHubContext<NegotiationHub> hubContext, ILogger<TripService> logger)
        {
            _unitOfWork = unitofWork;
            _mapServices = mapServices;
            _cache = cache;
            _hubContext = hubContext;
            _logger = logger;
        }

        // Fare calculation constants

        private const double BaseFare = 1000;
        private const double CostPerKm = 100;
        private const double CostPerMin = 100;
        private const double SurgeMultiplier = 1.3;
        private const double Surcharges = 300;

        private double CalculateFare(double distanceInMeters, double durationInSeconds)
        {
            double distanceKm = distanceInMeters / 1000.0;
            double durationMin = durationInSeconds / 60.0;

            double fare = BaseFare
                        + CostPerKm * distanceKm
                        + CostPerMin * durationMin;

            //fare *= SurgeMultiplier;
            fare += Surcharges;

            return Math.Round(fare / 100) * 100;

        }

        public async Task<NotificationResult> NotifyServiceProviders(TripDto booking)
        {
            var response = new NotificationResult();
            try
            {
                var providers = await _unitOfWork.Driver.GetAllByExpression(x =>  x.IsAvailable == true);
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
                        var distdura = await _mapServices.GetDistanceAndDurationAsync(provider.CurrentLocation, booking.StartLocation);
                        var result = await _mapServices.GetDistanceAsync(provider.CurrentLocation, booking.StartLocation);
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
                _logger.LogError(ex, "Error in NotifyServiceProvidersAsync for booking ID {BookingId}", booking.DriverId);
            }

            return response;
        }


        public Task<Response<string>> AddStop(Guid tripId, string location)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<bool>> CancelTrip(Guid tripId, Guid userId)
        {
            var trip = await _unitOfWork.Trip.GetAsync(tripId);
            trip.Status = TripStatus.Cancelled;
            trip.CancelledBy = userId;
            await _unitOfWork.Trip.UpdateAsync(trip);
            if (trip.DriverId != null)
            {
                var driver = await _unitOfWork.Driver.GetAsync((Guid)trip.DriverId);
                driver.IsAvailable = true;
                await _unitOfWork.Driver.UpdateAsync(driver);
            }
            return new Response<bool>
            {
                Message = "Trip cancelled successfully",
                Success = true,
                Data = true
            };
        }

        public async Task<Response<TripDto>> InitiateTrip(TripRequestDto tripRequestDto, Guid customerId)
        {
            var result = await _mapServices.GetDistanceAsync(tripRequestDto.StartLocation, tripRequestDto.EndLocation);
            var element = result.rows.FirstOrDefault()?.elements.FirstOrDefault();
            if (element == null || element.status != "OK")
            {
                return new Response<TripDto>
                {
                    Message = "Unable to calculate distance",
                    Success = false,
                    Data = null
                };
            }
            var distance = element.distance.text;
            var duration = element.duration.text;

            var (distanceMeters, durationSeconds) = _mapServices.GetDistanceAndDurationAsync(tripRequestDto.StartLocation, tripRequestDto.EndLocation).Result;
            var estimateFare = CalculateFare(distanceMeters, durationSeconds);

            var trip = new Trip
            {
                StartLocation = tripRequestDto.StartLocation,
                EndLocation = tripRequestDto.EndLocation,
                Distance = distance,
                Duration = duration,
                CustomerId = customerId,
                EstimatedFare = (decimal)estimateFare,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = customerId,
                Status = TripStatus.Initiated,
            };
            await _unitOfWork.Trip.AddAsync(trip);
            
            return new Response<TripDto>
            {
                Message = "Trip created successfully",
                Success = true,
                Data = new TripDto
                {
                    Id = trip.Id,
                    StartLocation = trip.StartLocation,
                    EndLocation = trip.EndLocation,
                    Distance = trip.Distance,
                    Duration = trip.Duration,
                    EstimateFare = trip.EstimatedFare,
                    CustomerId = trip.CustomerId
                }
            };
        }

        public async Task<Response<TripDto>> Book(Guid tripId)
        {
            var response = new Response<TripDto>();
            var trip = await _unitOfWork.Trip.GetAsync(tripId);
            if (trip == null)
            {
                response.Message = "Trip not found";
                response.Success = false;
                response.Data = null;
                return response;
            }
            trip.Status = TripStatus.Pending;
            await _unitOfWork.Trip.UpdateAsync(trip);
            response.Message = "Trip booked successfully";
            response.Success = true;
            var data = new TripDto
            {
                Id = trip.Id,
                StartLocation = trip.StartLocation,
                EndLocation = trip.EndLocation,
                Distance = trip.Distance,
                Duration = trip.Duration,
                CustomerId = trip.CustomerId,
                EstimateFare = trip.EstimatedFare
            };
            var notificationResult = await NotifyServiceProviders(data);
            data.ProvidersNotified = notificationResult.ProvidersNotified;
            response.Data = data;
            return response;
        }

        public async Task<Response<string>> EndTrip(Guid tripId)
        {
            var trip = await _unitOfWork.Trip.GetAsync(tripId);
            trip.Status = TripStatus.Completed;
            trip.Driver.IsAvailable = true;

            await _unitOfWork.Trip.UpdateAsync(trip);
            await _unitOfWork.Driver.UpdateAsync(trip.Driver);
            return new Response<string>
            {
                Message = "Trip ended successfully",
                Success = true,
                Data = trip.Price.ToString(),
            };
        }

        public async Task<Response<string>> GetDistanceBetweenDriverAndCustomer(Guid tripId, string driverLocation)
        {
            var trip = await _unitOfWork.Trip.GetAsync(x => x.Id == tripId);
            var result = await _mapServices.GetDistanceAsync(driverLocation, trip.StartLocation);
            var element = result.rows.FirstOrDefault()?.elements.FirstOrDefault();
            if (element == null || element.status != "OK")
            {
                return new Response<string>
                {
                    Message = "Unable to calculate distance",
                    Success = false,
                    Data = null
                };
            }
            return new Response<string>
            {
                Message = "Distance calculated successfully",
                Success = true,
                Data = $"Distance = {element.distance.text} and duration = {element.duration.text}"
            };

        }

        public async Task<Response<IList<TripDto>>> GetPendingTrips(string location)
        {
            var response = new Response<IList<TripDto>>();
            var trips = await _unitOfWork.Trip.GetAllByExpression(x => x.Status == TripStatus.Pending);
            if (trips == null || !trips.Any())
            {
                response.Message = "No pending trips found";
                response.Success = false;
                return response;
            }
            response.Data = new List<TripDto>();

            foreach (var x in trips)
            {
                var distandDura = await _mapServices.GetDistanceAndDurationAsync(location, x.StartLocation);

                var result = await _mapServices.GetDistanceAsync(location, x.StartLocation);
                var element = result.rows.FirstOrDefault()?.elements.FirstOrDefault();
                if (element == null || element.status != "OK")
                {
                    response.Message = "Failed to calculate distance and duration.";
                    response.Success = false;
                    return response;
                }
                var distance = element.distance.text;
                var duration = element.duration.text;

                if (distandDura.DistanceMeters < 2000)
                {
                    response.Data.Add(new TripDto
                    {
                        Id = x.Id,
                        StartLocation = x.StartLocation,
                        EndLocation = x.EndLocation,
                        Distance = x.Distance,
                        Duration = x.Duration,
                        CustomerId = x.CustomerId,
                        EstimateFare = x.EstimatedFare,
                        DistanceToPickup = distance,
                        DurationToPickup = duration,
                    });
                }
            }

            response.Message = "Pending trips retrieved successfully";
            response.Success = true;
            return response;
        }

        public Task<Response<TripDto>> GetTripById(Guid tripId)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<bool>> StartTrip(Guid tripId)
        {
            var trip = await _unitOfWork.Trip.GetAsync(tripId);
            trip.Status = TripStatus.InProgress;
            var driver = await _unitOfWork.Driver.GetAsync((Guid)trip.DriverId);
            driver.IsAvailable = false;
            await _unitOfWork.Trip.UpdateAsync(trip);
            await _unitOfWork.Driver.UpdateAsync(driver);
            return new Response<bool>
            {
                Message = "Trip started successfully",
                Success = true,
                Data = true
            };
        }

        public async Task<Response<bool>> RejectBooking(Guid tripId)
        {
            var response = new Response<bool>();
            var booking = await _unitOfWork.Trip.GetAsync(x => x.Id == tripId);
            if (booking == null)
            {
                response.Success = false;
                response.Message = "trip not found.";
                return response;
            }
            booking.Status = TripStatus.Accepted;
            await _unitOfWork.Trip.UpdateAsync(booking);
            response.Success = true;
            response.Message = "trip rejected successfully.";
            return response;
        }

        public async Task<Response<TripDto>> AcceptOffer(Guid tripId, Guid driverUserId, decimal offerAmount)
        {
            var response = new Response<TripDto>();
            var booking = await _unitOfWork.Trip.GetAsync(x => x.Id == tripId);
            if (booking == null)
            {
                response.Message = "Booking not found";
                response.Success = false;
                return response;
            }
            var driver = await _unitOfWork.Driver.GetAsync(x => x.UserId == driverUserId);

            booking.DriverId = driver.Id;
            booking.Price = offerAmount;
            booking.Status = TripStatus.Accepted;
            await _unitOfWork.Trip.UpdateAsync(booking);
            response.Message = "Offer accepted successfully";
            response.Success = true;


            var res = await _mapServices.GetDistanceAsync(driver.CurrentLocation, booking.StartLocation);
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
            response.Data = new TripDto
            {
                Id = booking.Id,
                StartLocation = booking.StartLocation,
                EndLocation = booking.EndLocation,
                Distance = booking.Distance,
                Duration = booking.Duration,
                CustomerId = booking.CustomerId,
                EstimateFare = booking.EstimatedFare,
                DriverId = driver.Id,
                Price = booking.Price,
                DistanceToPickup = distanceToPickupLocation,
                DurationToPickup = durationToPickupLocation
            };
            driver.IsAvailable = false;
            await _unitOfWork.Driver.UpdateAsync(driver);
            return response;

        }

        public async Task<Response<TripDto>> RaiseFare(Guid tripId, decimal newFare)
        {
            var response = new Response<TripDto>();
            var booking = await _unitOfWork.Trip.GetAsync(x => x.Id == tripId);
            if (booking == null)
            {
                response.Message = "Booking not found";
                response.Success = false;
                return response;
            }
            if (newFare <= booking.EstimatedFare)
            {
                response.Success = false;
                response.Message = "You can only increase the price.";
                return response;
            }
            booking.EstimatedFare = newFare;
            await _unitOfWork.Trip.UpdateAsync(booking);
            response.Message = "Price raised successfully";
            response.Success = true;
            var data = new TripDto
            {
                Id = booking.Id,
                StartLocation = booking.StartLocation,
                EndLocation = booking.EndLocation,
                Distance = booking.Distance,
                Duration = booking.Duration,
                CustomerId = booking.CustomerId,
                EstimateFare = booking.EstimatedFare
            };
            var notify = await NotifyServiceProviders(response.Data);
            data.ProvidersNotified = notify.ProvidersNotified;
            response.Data = data;
            return response;
        }

        public async Task<Response<IList<TripDto>>> GetTrips(Guid userId)
        {
            var response = new Response<IList<TripDto>>();
            var trips = await _unitOfWork.Trip.GetTrips(x => x.Driver.UserId == userId || x.CustomerId == userId && x.Status != TripStatus.Initiated);
            if (trips == null)
            {
                response.Message = "No trips";
                response.Success = false;
                return response;
            }

            response.Message = "Trip found";
            response.Success = true;
            response.Data = trips.Select(x => new TripDto
            {
                Id = x.Id,
                StartLocation = x.StartLocation,
                EndLocation = x.EndLocation,
                Distance = x.Distance,
                Duration = x.Duration,
                CustomerId = x.CustomerId,
                EstimateFare = x.EstimatedFare,
                Price = x.Price,
                DistanceToPickup = x.DistanceToPickup,
                DurationToPickup = x.DurationToPickup
            }).ToList();
            throw new NotImplementedException();
        }
    }
}
