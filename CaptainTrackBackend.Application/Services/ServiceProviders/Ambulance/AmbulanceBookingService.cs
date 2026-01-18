using CaptainTrackBackend.Application.Abstraction.Interface.Maps;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.Ambulance;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.MapModels;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.Ambulance;
using CaptainTrackBackend.Domain.Entities;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.Ambulance;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.WaterSupply;
using CaptainTrackBackend.Domain.Enum;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Services.ServiceProviders.Ambulance
{
    public class AmbulanceBookingService : IAmbulanceBookingService
    {
        private readonly IUnitofWork _unitOfWork;
        private readonly IMapServices _mapServices;
        private readonly IHubContext<NegotiationHub> _hubContext;
        private readonly ILogger<AmbulanceBookingService> _logger;
        public AmbulanceBookingService(IUnitofWork unitofWork, IMapServices mapServices, IHubContext<NegotiationHub> hubContext, ILogger<AmbulanceBookingService> logger)
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

        public async Task<NotificationResult> NotifyAmbulance(AmbulanceBookingDto booking)
        {
            var response = new NotificationResult();         
            try
            {
                var providers = await _unitOfWork.Ambulance.GetAllByExpression(x => x.ServiceProviderRole == ServiceProviderRole.Freelancer && x.IsAvailable == true);
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
                _logger.LogError(ex, "Error in NotifyServiceProvidersAsync for booking ID {BookingId}",booking.AmbulanceBookingId);
            }

            return response;   
        }

        public async Task<Response<AmbulanceBookingDto>> InitiateAmbulanceBookingAsync(Guid customerId, AmbulanceBookingRequest request, Guid? ambulanceUserId = null)
        {
            var response = new Response<AmbulanceBookingDto>();
            AmbulanceCompany ambulance = null;
            if (ambulanceUserId != null)
            {
                ambulance = await _unitOfWork.Ambulance.GetAsync(x => x.UserId == ambulanceUserId);
                if (ambulance == null)
                {
                    response.Message = "Ambulance not found";
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

            AmbulanceBooking booking = new AmbulanceBooking();
            if (ambulance != null)
            {
                var res = await _mapServices.GetDistanceAsync(ambulance.AddressorLocation, request.PickupLocation);
                var elem = res.rows.FirstOrDefault()?.elements.FirstOrDefault();
                var distanceToPickupLocation = elem?.distance.text;
                var durationToPickupLocation = elem?.duration.text;

                booking.CustomerId = customerId;
                booking.AmbulanceCompanyId = ambulance.Id;
                booking.PickupLocation = request.PickupLocation;
                booking.Destination = request.Destination;
                booking.Distance = distance;
                booking.Duration = duration;
                booking.DistanceToPickupLocation = distanceToPickupLocation;
                booking.DurationToPickupLocation = durationToPickupLocation;
                booking.EstimatedPrice = (decimal)CalculateFare(ambulance.Price, distanceMeters);
            }
            else
            {
                var price = await _unitOfWork.Context.Prices.FirstOrDefaultAsync();

                booking.CustomerId = customerId;
                booking.PickupLocation = request.PickupLocation;
                booking.Destination = request.Destination;
                booking.Distance = distance;
                booking.Duration = duration;
                booking.EstimatedPrice = (decimal)CalculateFare(price.AmbulancePrice, distanceMeters);
            }

            booking.ServiceStatus = ServiceStatus.Init;
            await _unitOfWork.AmbulanceBooking.AddAsync(booking);

            response.Message = "Booking Initiated succecfully";
            response.Success = true;
            response.Data = new AmbulanceBookingDto
            {
                AmbulanceBookingId = booking.Id,
                CustomerId = booking.CustomerId,
                AmbulanceId = booking.AmbulanceCompanyId,
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
            return response;
        }

        public async Task<Response<AmbulanceBookingDto>> Book(Guid bookingId)
        {
            var response = new Response<AmbulanceBookingDto>();
            var booking = await _unitOfWork.AmbulanceBooking.GetAsync(x => x.Id == bookingId);
            if (booking == null)
            {
                response.Message = "Booking not found";
                response.Success = false;
                return response;
            }
            if (booking.AmbulanceCompanyId != null)
            {
                booking.ServiceStatus = ServiceStatus.Booked;
                booking.TotalPrice = booking.EstimatedPrice; 
                await _unitOfWork.AmbulanceBooking.UpdateAsync(booking);
                response.Message = "Booking Sent";
                response.Success = true;
                response.Data = new AmbulanceBookingDto
                {
                    AmbulanceBookingId = booking.Id,
                    CustomerId = booking.CustomerId,
                    AmbulanceId = booking.AmbulanceCompanyId,
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
            else
            {
                booking.ServiceStatus = ServiceStatus.Pending;
                await _unitOfWork.AmbulanceBooking.UpdateAsync(booking);
                response.Message = "Booking Sent, waiting for ambulance to accept";
                response.Success = true;
                var bookingDto = new AmbulanceBookingDto
                {
                    AmbulanceBookingId = booking.Id,
                    CustomerId = booking.CustomerId,
                    AmbulanceId = booking.AmbulanceCompanyId,
                    PickupLocation = booking.PickupLocation,
                    Destination = booking.Destination,
                    Distance = booking.Distance,
                    Duration = booking.Duration,
                    EstimatedPrice = booking.EstimatedPrice,
                    ServiceStatus = booking.ServiceStatus.ToString()
                };
                var not = await NotifyAmbulance(bookingDto);
                bookingDto.ProvidersNotified = not.ProvidersNotified;
                response.Data = bookingDto;
            }
            return response;
        }

        public async Task<Response<AmbulanceBookingDto>> GetBookingDetailsAsync(Guid ambulanceBookingId)
        {
            var response = new Response<AmbulanceBookingDto>();
            var booking = await _unitOfWork.AmbulanceBooking.GetAsync(x => x.Id == ambulanceBookingId);
            if (booking == null)
            {
                response.Message = "Booking not found";
                response.Success = false;
                return response;
            }
            var bookingDto = new AmbulanceBookingDto
            {
                AmbulanceBookingId = booking.Id,
                CustomerId = booking.CustomerId,
                AmbulanceId = booking.AmbulanceCompanyId,
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
            response.Data = bookingDto;
            response.Message = "Booking details retrieved successfully";
            response.Success = true;
            return response;
        }

        public async Task<Response<IEnumerable<AmbulanceBookingDto>>> GetBookingsByUserIdAsync(Guid userId)
        {
            var response = new Response<IEnumerable<AmbulanceBookingDto>>();
            var bookings = await _unitOfWork.AmbulanceBooking.GetBookings(x => x.CustomerId == userId || x.AmbulanceCompany.UserId == userId && x.ServiceStatus != ServiceStatus.Init);
            if (bookings == null || !bookings.Any())
            {
                response.Message = "No bookings found for this user";
                response.Success = false;
                return response;
            }
            var bookingDtos = bookings.Select(booking => new AmbulanceBookingDto
            {
                AmbulanceBookingId = booking.Id,
                CustomerId = booking.CustomerId,
                AmbulanceId = booking.AmbulanceCompanyId,
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
            response.Data = bookingDtos;
            response.Message = "Bookings retrieved successfully";
            response.Success = true;
            return response;
        }

        public async Task<Response<IEnumerable<AmbulanceBookingDto>>> GetPendings(string location)
        {
            var response = new Response<IEnumerable<AmbulanceBookingDto>>();
            var bookings = await _unitOfWork.AmbulanceBooking.GetAllByExpression(x => x.ServiceStatus == ServiceStatus.Pending);
            if (bookings == null)
            {
                response.Message = "No pending bookings";
                response.Success = false;
                return response;
            }

            var bookingsDto = new List<AmbulanceBookingDto>();
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
                    bookingsDto.Add(new AmbulanceBookingDto
                    {
                        AmbulanceBookingId = booking.Id,
                        CustomerId = booking.CustomerId,
                        AmbulanceId = booking.AmbulanceCompanyId,
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
                }
            }
            response.Message = "Pending Bookings gotten";
            response.Success = true;
            response.Data = bookingsDto;
            return response;
        }

        public Task<Response<bool>> CancelBookingAsync(Guid bookingId)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<AmbulanceBookingDto>> AcceptOffer(Guid bookingId,Guid ambulanceUserId, decimal offerAmount)
        {
            var response = new Response<AmbulanceBookingDto>();
            var ambulance = await _unitOfWork.Ambulance.GetAsync(x => x.UserId == ambulanceUserId);
            if (ambulance == null)
            {
                response.Message = "Ambulance not found";
                response.Success = false;
                return response;
            }
            var booking = await _unitOfWork.AmbulanceBooking.GetAsync(x => x.Id == bookingId);
            if (booking == null)
            {
                response.Message = "Booking not found";
                response.Success = false;
                return response;
            }

            booking.AmbulanceCompanyId = ambulance.Id;
            booking.TotalPrice = offerAmount;
            booking.ServiceStatus = ServiceStatus.Booked;
            await _unitOfWork.AmbulanceBooking.UpdateAsync(booking);
            response.Message = "Offer accepted successfully";
            response.Success = true;


            var res = await _mapServices.GetDistanceAsync(ambulance.AddressorLocation, booking.PickupLocation);
            var elem = res.rows.FirstOrDefault()?.elements.FirstOrDefault();
            var distanceToPickupLocation = elem?.distance.text;
            var durationToPickupLocation = elem?.duration.text;
            if (distanceToPickupLocation == null || durationToPickupLocation == null)
            {
                response.Message = "Failed to calculate distance or duration.";
                response.Success = false;
                return response;
            }
            response.Data = new AmbulanceBookingDto
            {
                AmbulanceBookingId = booking.Id,
                CustomerId = booking.CustomerId,
                AmbulanceId = ambulance.Id,
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
            return response;
        }    

        public async Task<Response<bool>> AcceptBookingAsync(Guid bookingId)
        {
            var response = new Response<bool>();
            var booking = await _unitOfWork.AmbulanceBooking.GetAsync(x => x.Id == bookingId);
            if (booking == null)
            {
                response.Message = "Booking not found";
                response.Success = false;
                return response;
            }
            booking.ServiceStatus = ServiceStatus.InProgress;
            await _unitOfWork.AmbulanceBooking.UpdateAsync(booking);
            response.Message = "Booking accepted successfully";
            response.Success = true;
            response.Data = true;
            return response;
        }

        public async Task<Response<AmbulanceBookingDto>> RaisePrice(Guid bookingId, decimal newPrice)
        {
            var response = new Response<AmbulanceBookingDto>();
            var booking = await _unitOfWork.AmbulanceBooking.GetAsync(x => x.Id == bookingId);
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
            await _unitOfWork.AmbulanceBooking.UpdateAsync(booking);
            response.Message = "Price raised successfully";
            response.Success = true;
            var data = new AmbulanceBookingDto
            {
                AmbulanceBookingId = booking.Id,
                CustomerId = booking.CustomerId,
                AmbulanceId = booking.AmbulanceCompanyId,
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
            var notify = await NotifyAmbulance(response.Data);
            data.ProvidersNotified = notify.ProvidersNotified;
            response.Data = data;
            return response;
        }

    }
}
 