using CaptainTrackBackend.Application.Abstraction.Interface.Maps;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.CarWash;
using CaptainTrackBackend.Application.DTO.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.CarWash;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Domain.Enum;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CaptainTrackBackend.Application.Services.ServiceProviders.DryCleaning
{
    public class DryCleaningService : IDryCleaningService
    {
        IUnitofWork _unitOfWork;
        ApplicationDbContext _context;
        private readonly IMapServices _mapServices;
        private readonly IHubContext<NegotiationHub> _hubContext;
        private readonly ILogger<DryCleaningService> _logger;

        public DryCleaningService(IUnitofWork unitofWork, ApplicationDbContext context, IMapServices mapServices,
            IHubContext<NegotiationHub> hubContext, ILogger<DryCleaningService> logger)
        {
            _unitOfWork = unitofWork;
            _context = context;
            _mapServices = mapServices;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task<NotificationResult> NotifyServiceProviders(DryCleaningDto booking)
        {
            var response = new NotificationResult();
            try
            {
                var providers = await _unitOfWork.DryCleaner.GetAllByExpression(x => x.ServiceProviderRole == ServiceProviderRole.Freelancer && x.IsAvailable == true);
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
                        var distdura = await _mapServices.GetDistanceAndDurationAsync(provider.AddressorLocation, booking.Location);
                        var result = await _mapServices.GetDistanceAsync(provider.AddressorLocation, booking.Location);
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


                            await _hubContext.Clients.User(provider.UserId.ToString())
                                .SendAsync("ReceivePendingBooking", booking);
                            await _hubContext.Clients.Group(provider.UserId.ToString())
                                .SendAsync("ReceivePendingBooking", booking);
                            response.ProvidersNotified++;
                            _logger.LogInformation($"Notified provider {provider.UserId}");
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
                _logger.LogError(ex, "Error in NotifyServiceProvidersAsync for booking ID {BookingId}", booking.DrycleaningId);
            }

            return response;
        }


        public async Task<Response<DryCleaningDto>> Book(Guid dryCleaningId)
        {    
            var response = new Response<DryCleaningDto>();
            var dryCleaning = await _unitOfWork.DryCleaning.GetByIdWithDetailsAsync(dryCleaningId);
            if (dryCleaning == null)
            {
                response.Success = false;
                response.Message = "Dry cleaning booking not found.";
                return response;
            }
            if(dryCleaning.DryCleanerId != null)
            {
                dryCleaning.Status = ServiceStatus.Booked;
                dryCleaning.TotalAmount = dryCleaning.EstimateTotalAMount;
                await _unitOfWork.DryCleaning.UpdateAsync(dryCleaning);
                response.Message = "Dry cleaning booking successful";
                response.Success = true;
                response.Data = new DryCleaningDto
                {
                    DrycleaningId = dryCleaning.Id,
                    CustomerId = dryCleaning.CustomerId,
                    DryCleanerId = dryCleaning.DryCleanerId,
                    PackageId = dryCleaning.PackageId,
                    DeliveryDate = dryCleaning.DeliveryDate,
                    Status = dryCleaning.Status.ToString(),
                    TotalAmount = dryCleaning.TotalAmount,
                    DryCleaningItems = dryCleaning.DryCleaningItems.Select(x => new DryCleaningItemDto
                    {
                        ItemId = x.DryCleanerLaundryItemId,
                        ItemName = x.LaundryItem?.Name ?? x.DryCleanerLaundryItem?.LaundryItem?.Name,
                        Quantity = x.Quantity,
                        TotalPrice = x.TotalPrice
                    }).ToList()
                };
            }
            else
            {
                dryCleaning.Status = ServiceStatus.Pending;
                await _unitOfWork.DryCleaning.UpdateAsync(dryCleaning);
                var bookingDto = new DryCleaningDto
                {
                    DrycleaningId = dryCleaning.Id,
                    CustomerId = dryCleaning.CustomerId,
                    PackageId = dryCleaning.PackageId,
                    DeliveryDate = dryCleaning.DeliveryDate,
                    Status = dryCleaning.Status.ToString(),
                    EstimateTotalAmount = dryCleaning.EstimateTotalAMount,
                    DryCleaningItems = dryCleaning.DryCleaningItems.Select(x => new DryCleaningItemDto
                    {
                        ItemId = x.LaundryItemId,
                        ItemName = x.LaundryItem?.Name ?? x.DryCleanerLaundryItem?.LaundryItem?.Name,
                        Quantity = x.Quantity,
                        TotalPrice = x.TotalPrice
                    }).ToList()
                };
                response.Message = "Dry cleaning booking initialized successfully. Waiting for service provider to accept.";
                response.Success = true;
                var not = await NotifyServiceProviders(bookingDto);
                bookingDto.ProvidersNotified = not.ProvidersNotified;
                response.Data = bookingDto;
            }
            return response;
        }

        public async Task<Response<List<DryCleaningDto>>> GetPending(string location)
        {
            var response = new Response<List<DryCleaningDto>>();
            var dryCleanings = await _unitOfWork.DryCleaning.GetAllByExpressionAsync(x => x.Status == ServiceStatus.Pending);
            if (dryCleanings == null || !dryCleanings.Any())
            {
                response.Success = false;
                response.Message = "No pending dry cleaning bookings found.";
                return response;
            }

            var bookings = new List<DryCleaningDto>();
            foreach (var x in dryCleanings)
            {
                var distandDura = await _mapServices.GetDistanceAndDurationAsync(location, x.CustomerLocation);

                var result = await _mapServices.GetDistanceAsync(location, x.CustomerLocation);
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
                    bookings.Add(new DryCleaningDto
                    {
                        DrycleaningId = x.Id,
                        CustomerId = x.CustomerId,
                        CustomerName = x.Customer.FirstName + " " + x.Customer.LastName,
                        Location = x.CustomerLocation,
                        PackageId = x.PackageId,
                        DeliveryDate = x.DeliveryDate,
                        Status = x.Status.ToString(),
                        EstimateTotalAmount = x.EstimateTotalAMount,
                        DryCleaningItems = x.DryCleaningItems.Select(item => new DryCleaningItemDto
                        {
                            Id = item.Id,
                            ItemId = item.LaundryItemId ?? item.DryCleanerLaundryItemId,
                            ItemName = item.LaundryItem?.Name ?? item.DryCleanerLaundryItem?.LaundryItem?.Name,
                            Quantity = item.Quantity,
                            TotalPrice = item.TotalPrice
                        }).ToList()
                    });
                }
            }

            response.Success = true;
            response.Message = "Pending dry cleaning bookings retrieved successfully.";
            response.Data = bookings;
            return response;
        }

        public async Task<Response<DryCleaningDto>> InitBook(Guid customerId, Guid dryCleanerUserid, DryCleaningRequestDto dryCleaningrequest)
        {
            var dryCleaner = await _unitOfWork.DryCleaner.GetAsync(x => x.UserId == dryCleanerUserid);
            var dryClean = new DryClean
            {
                CustomerId = customerId,
                DryCleanerId = dryCleaner.Id,
                PackageId = dryCleaningrequest.PackageId,
                Status = ServiceStatus.Init,
                DeliveryDate = dryCleaningrequest.DeliveryDate,
                CustomerLocation = dryCleaningrequest.Location,
                EstimateTotalAMount = 0,
                DryCleaningItems = new List<DryCleaningItem>()
            };
            foreach (var item in dryCleaningrequest.DryCleaningItems)
            {
                var laundryItem = await _context.DryCleanerLaundryItems.FirstOrDefaultAsync(x => x.Id == item.ItemId);
                if (laundryItem == null)
                {
                    return new Response<DryCleaningDto>
                    {
                        Success = false,
                        Message = $"DryCleanerLaundryItem not found."
                    };
                }
                var dryCleaningItem = new DryCleaningItem
                {
                    DryCleanerLaundryItemId = item.ItemId,
                    Quantity = item.Quantity,
                    TotalPrice = laundryItem.Price * item.Quantity 
                };

                dryClean.DryCleaningItems.Add(dryCleaningItem);
                dryClean.EstimateTotalAMount += dryCleaningItem.TotalPrice;
            }

            var package = await _unitOfWork.LaundryPackage.GetAsync(x => x.Id == dryCleaningrequest.PackageId);
            dryClean.EstimateTotalAMount += package.ExtraCharge;
            await _unitOfWork.DryCleaning.AddAsync(dryClean);

            var bookingDto = new DryCleaningDto
            {
                DrycleaningId = dryClean.Id,
                CustomerId = dryClean.CustomerId,
                DryCleanerId = dryClean.DryCleanerId,
                PackageId = dryClean.PackageId,
                DeliveryDate = dryClean.DeliveryDate,
                Status = dryClean.Status.ToString(),
                TotalAmount = dryClean.TotalAmount,
                DryCleaningItems = dryClean.DryCleaningItems.Select(x => new DryCleaningItemDto
                {
                    Id = x.Id,
                    ItemId = x.DryCleanerLaundryItemId,
                    ItemName = x.LaundryItem?.Name ?? x.DryCleanerLaundryItem?.LaundryItem?.Name,
                    Quantity = x.Quantity,
                    TotalPrice = x.TotalPrice
                }).ToList()
            };

            // Notify the chosen provider in real-time
            await _hubContext.Clients.User(dryCleanerUserid.ToString())
                .SendAsync("ReceivePendingBooking", bookingDto);
            await _hubContext.Clients.Group(dryCleanerUserid.ToString())
                .SendAsync("ReceivePendingBooking", bookingDto);

            return new Response<DryCleaningDto>
            {
                Message = "Dry cleaning booking initialized successfully",
                Success = true,
                Data = bookingDto
            };
        }

        public async Task<Response<DryCleaningDto>> InitBookFreelancer(Guid customerId, DryCleaningRequestDto dryCleaningrequest)
        {
            var dryClean = new DryClean
            {
                CustomerId = customerId,
                PackageId = dryCleaningrequest.PackageId,
                Status = ServiceStatus.Init,
                DeliveryDate = dryCleaningrequest.DeliveryDate,
                CustomerLocation = dryCleaningrequest.Location,
                
                EstimateTotalAMount = 0,
                DryCleaningItems = new List<DryCleaningItem>()
            };
            foreach (var item in dryCleaningrequest.DryCleaningItems)
            {
                var laundryItem = await _unitOfWork.LaundryItem.GetAsync(x => x.Id == item.ItemId);
                if (laundryItem == null)
                {
                    return new Response<DryCleaningDto>
                    {
                        Success = false,
                        Message = $"LaundryItem with ID {item.ItemId} not found."
                    };
                }

                var dryCleaningItem = new DryCleaningItem
                {
                    LaundryItemId = item.ItemId,
                    Quantity = item.Quantity,
                    TotalPrice = laundryItem.Price * item.Quantity
                };

                dryClean.DryCleaningItems.Add(dryCleaningItem);
                dryClean.EstimateTotalAMount += dryCleaningItem.TotalPrice;
            }
            var package = await _unitOfWork.LaundryPackage.GetAsync(x => x.Id == dryCleaningrequest.PackageId);
            await _unitOfWork.DryCleaning.AddAsync(dryClean);
            return new Response<DryCleaningDto>
            {
                Message = "Dry cleaning booking initialized successfully",
                Success = true,
                Data = new DryCleaningDto
                {
                    DrycleaningId = dryClean.Id,
                    CustomerId = dryClean.CustomerId,
                    PackageId = dryClean.PackageId,
                    DeliveryDate = dryClean.DeliveryDate,
                    Status = dryClean.Status.ToString(),
                    EstimateTotalAmount = dryClean.EstimateTotalAMount,
                    DryCleaningItems = dryClean.DryCleaningItems.Select(x => new DryCleaningItemDto
                    {
                        Id = x.Id,
                        ItemId = x.LaundryItemId,
                        ItemName = x.LaundryItem?.Name ?? x.DryCleanerLaundryItem?.LaundryItem?.Name,
                        Quantity = x.Quantity,
                        TotalPrice = x.TotalPrice
                    }).ToList()
                }
            };
        }

        public async Task<Response<List<DryCleaningDto>>> GetBookings(Guid userId)
        {
/*            var dryCleaner = await _unitOfWork.DryCleaner.GetAsync(x => x.UserId == dryCleanerUserId);
            if (dryCleaner == null)
            {
                return new Response<List<DryCleaningDto>>
                {
                    Success = false,
                    Message = "Dry cleaner not found."
                };
            }*/
            var dryCleanings = await _unitOfWork.DryCleaning.GetAllByExpressionAsync(x => x.DryCleaner.UserId == userId || x.CustomerId == userId && x.Status != ServiceStatus.Init);
            if (dryCleanings == null || !dryCleanings.Any())
            {
                return new Response<List<DryCleaningDto>>
                {
                    Success = false,
                    Message = "No dry cleaning bookings found."
                };
            }
            var dryCleaningDtos = dryCleanings.Select(x => new DryCleaningDto
            {
                DrycleaningId = x.Id,
                CustomerId = x.CustomerId,
                CustomerName = x.Customer.FirstName + " " + x.Customer.LastName,
                Location = x.CustomerLocation,
                PackageId = x.PackageId,
                DeliveryDate = x.DeliveryDate,
                Status = x.Status.ToString(),
                EstimateTotalAmount = x.EstimateTotalAMount,
                DryCleaningItems = x.DryCleaningItems.Select(item => new DryCleaningItemDto
                {
                    Id = item.Id,
                    ItemId = item.LaundryItemId ?? item.DryCleanerLaundryItemId,
                    ItemName = item.LaundryItem?.Name ?? item.DryCleanerLaundryItem?.LaundryItem?.Name,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice
                }).ToList()
            }).ToList();
            return new Response<List<DryCleaningDto>>
            {
                Success = true,
                Message = "Bookings retrieved successfully.",
                Data = dryCleaningDtos
            };
        }

        public async Task<Response<bool>> AccepBooking(Guid dryCleaningId)
        {
            var response = new Response<bool>();
            var dryCleaning = await _unitOfWork.DryCleaning.GetByIdWithDetailsAsync(dryCleaningId);
            if (dryCleaning == null)
            {
                response.Success = false;
                response.Message = "Dry cleaning booking not found.";
                return response;
            }
            dryCleaning.Status = ServiceStatus.InProgress;
            await _unitOfWork.DryCleaning.UpdateAsync(dryCleaning);

            // Notify the customer in real-time that the provider accepted
            var bookingDto = new DryCleaningDto
            {
                DrycleaningId = dryCleaning.Id,
                CustomerId = dryCleaning.CustomerId,
                DryCleanerId = dryCleaning.DryCleanerId,
                PackageId = dryCleaning.PackageId,
                DeliveryDate = dryCleaning.DeliveryDate,
                Status = dryCleaning.Status.ToString(),
                TotalAmount = dryCleaning.TotalAmount,
                EstimateTotalAmount = dryCleaning.EstimateTotalAMount,
                DistanceToPickupLocation = dryCleaning.DistancetoLocation,
                DurationToPickupLocation = dryCleaning.DuraiontoLocation,
                DryCleaningItems = dryCleaning.DryCleaningItems.Select(x => new DryCleaningItemDto
                {
                    Id = x.Id,
                    ItemId = x.LaundryItemId ?? x.DryCleanerLaundryItemId,
                    ItemName = x.LaundryItem?.Name ?? x.DryCleanerLaundryItem?.LaundryItem?.Name,
                    Quantity = x.Quantity,
                    TotalPrice = x.TotalPrice
                }).ToList()
            };
            await _hubContext.Clients.User(dryCleaning.CustomerId.ToString())
                .SendAsync("BookingStatusUpdated", bookingDto);
            await _hubContext.Clients.Group(dryCleaning.CustomerId.ToString())
                .SendAsync("BookingStatusUpdated", bookingDto);

            // Notify the dry cleaner that they successfully accepted the booking
            if (dryCleaning.DryCleaner != null)
            {
                await _hubContext.Clients.User(dryCleaning.DryCleaner.UserId.ToString())
                    .SendAsync("BookingStatusUpdated", bookingDto);
                await _hubContext.Clients.Group(dryCleaning.DryCleaner.UserId.ToString())
                    .SendAsync("BookingStatusUpdated", bookingDto);
            }

            response.Success = true;
            response.Message = "Dry cleaning booking accepted successfully.";
            return response;
        }

        public async Task<Response<bool>> RejectBooking(Guid dryCleaningId)
        {
            var response = new Response<bool>();
            var dryCleaning = await _unitOfWork.DryCleaning.GetByIdWithDetailsAsync(dryCleaningId);
            if (dryCleaning == null)
            {
                response.Success = false;
                response.Message = "Dry cleaning booking not found.";
                return response;
            }
            dryCleaning.Status = ServiceStatus.Rejected;
            await _unitOfWork.DryCleaning.UpdateAsync(dryCleaning);

            // Notify the customer in real-time that the provider rejected
            var bookingDto = new DryCleaningDto
            {
                DrycleaningId = dryCleaning.Id,
                CustomerId = dryCleaning.CustomerId,
                DryCleanerId = dryCleaning.DryCleanerId,
                PackageId = dryCleaning.PackageId,
                DeliveryDate = dryCleaning.DeliveryDate,
                Status = dryCleaning.Status.ToString(),
                EstimateTotalAmount = dryCleaning.EstimateTotalAMount,
                DryCleaningItems = dryCleaning.DryCleaningItems.Select(x => new DryCleaningItemDto
                {
                    Id = x.Id,
                    ItemId = x.LaundryItemId ?? x.DryCleanerLaundryItemId,
                    ItemName = x.LaundryItem?.Name ?? x.DryCleanerLaundryItem?.LaundryItem?.Name,
                    Quantity = x.Quantity,
                    TotalPrice = x.TotalPrice
                }).ToList()
            };
            await _hubContext.Clients.User(dryCleaning.CustomerId.ToString())
                .SendAsync("BookingStatusUpdated", bookingDto);
            await _hubContext.Clients.Group(dryCleaning.CustomerId.ToString())
                .SendAsync("BookingStatusUpdated", bookingDto);

            // Notify the dry cleaner that their rejection was processed
            if (dryCleaning.DryCleaner != null)
            {
                await _hubContext.Clients.User(dryCleaning.DryCleaner.UserId.ToString())
                    .SendAsync("BookingStatusUpdated", bookingDto);
                await _hubContext.Clients.Group(dryCleaning.DryCleaner.UserId.ToString())
                    .SendAsync("BookingStatusUpdated", bookingDto);
            }

            response.Success = true;
            response.Message = "Dry cleaning booking rejected successfully.";
            return response;
        }

        public async Task<Response<bool>> CancelBooking(Guid dryCleaningId)
        {
            var response = new Response<bool>();
            var dryCleaning = await _unitOfWork.DryCleaning.GetByIdWithDetailsAsync(dryCleaningId);
            if (dryCleaning == null)
            {
                response.Success = false;
                response.Message = "Dry cleaning booking not found.";
                return response;
            }
            dryCleaning.Status = ServiceStatus.Cancelled;
            await _unitOfWork.DryCleaning.UpdateAsync(dryCleaning);

            // Notify both the customer and the provider in real-time
            var bookingDto = new DryCleaningDto
            {
                DrycleaningId = dryCleaning.Id,
                CustomerId = dryCleaning.CustomerId,
                DryCleanerId = dryCleaning.DryCleanerId,
                PackageId = dryCleaning.PackageId,
                DeliveryDate = dryCleaning.DeliveryDate,
                Status = dryCleaning.Status.ToString(),
                EstimateTotalAmount = dryCleaning.EstimateTotalAMount,
                DryCleaningItems = dryCleaning.DryCleaningItems.Select(x => new DryCleaningItemDto
                {
                    Id = x.Id,
                    ItemId = x.LaundryItemId ?? x.DryCleanerLaundryItemId,
                    ItemName = x.LaundryItem?.Name ?? x.DryCleanerLaundryItem?.LaundryItem?.Name,
                    Quantity = x.Quantity,
                    TotalPrice = x.TotalPrice
                }).ToList()
            };

            // Notify the customer via both User and Group targeting
            await _hubContext.Clients.User(dryCleaning.CustomerId.ToString())
                .SendAsync("BookingStatusUpdated", bookingDto);
            await _hubContext.Clients.Group(dryCleaning.CustomerId.ToString())
                .SendAsync("BookingStatusUpdated", bookingDto);

            // Notify the provider (if one was assigned)
            if (dryCleaning.DryCleanerId != null && dryCleaning.DryCleaner != null)
            {
                await _hubContext.Clients.User(dryCleaning.DryCleaner.UserId.ToString())
                    .SendAsync("BookingStatusUpdated", bookingDto);
                await _hubContext.Clients.Group(dryCleaning.DryCleaner.UserId.ToString())
                    .SendAsync("BookingStatusUpdated", bookingDto);
            }

            response.Success = true;
            response.Message = "Dry cleaning booking cancelled successfully.";
            return response;
        }

        public async Task<Response<DryCleaningDto>> AcceptOffer(Guid bookingId, Guid dryCleanerUserId, decimal offerAmount)
        {
            var response = new Response<DryCleaningDto>();
            var booking = await _unitOfWork.DryCleaning.GetByIdWithDetailsAsync(bookingId);
            if (booking == null)
            {
                response.Success = false;
                response.Message = "Booking not found.";
                return response;
            }
            var dryCleaner = await _unitOfWork.DryCleaner.GetAsync(x => x.UserId == dryCleanerUserId);
            if (dryCleaner == null)
            {
                response.Success = false;
                response.Message = "Dry cleaner not found.";
                return response;
            }

            booking.DryCleanerId = dryCleaner.Id;
            booking.Status = ServiceStatus.Booked;
            booking.TotalAmount = offerAmount;


            var res = await _mapServices.GetDistanceAsync(dryCleaner.AddressorLocation, booking.CustomerLocation);
            if (res?.rows == null || !res.rows.Any())
            {
                response.Message = "Failed to get distance from maps (no result). Check provider and customer addresses.";
                response.Success = false;
                return response;
            }
            var elem = res.rows.FirstOrDefault()?.elements?.FirstOrDefault();
            var distanceToPickupLocation = elem?.distance?.text;
            var durationToPickupLocation = elem?.duration?.text;
            if (string.IsNullOrEmpty(distanceToPickupLocation) || string.IsNullOrEmpty(durationToPickupLocation))
            {
                response.Message = "Failed to calculate distance or duration. The maps service may have returned an error for the given addresses.";
                response.Success = false;
                return response;
            }

            booking.DistancetoLocation = distanceToPickupLocation;
            booking.DuraiontoLocation = durationToPickupLocation;

            await _unitOfWork.DryCleaning.UpdateAsync(booking);

            response.Success = true;
            response.Message = "Offer accepted successfully.";
            response.Data = new DryCleaningDto
            {
                DrycleaningId = booking.Id,
                CustomerId = booking.CustomerId,
                DryCleanerId = booking.DryCleanerId,
                PackageId = booking.PackageId,
                DeliveryDate = booking.DeliveryDate,
                Status = booking.Status.ToString(),
                TotalAmount = booking.TotalAmount,
                DistanceToPickupLocation = booking.DistancetoLocation,
                DurationToPickupLocation = booking.DuraiontoLocation,
                DryCleaningItems = booking.DryCleaningItems.Select(x => new DryCleaningItemDto
                {
                    Id = x.Id,
                    ItemId = x.LaundryItemId ?? x.DryCleanerLaundryItemId,
                    ItemName = x.LaundryItem?.Name ?? x.DryCleanerLaundryItem?.LaundryItem?.Name,
                    Quantity = x.Quantity,
                    TotalPrice = x.TotalPrice
                }).ToList()
            };

            // Notify the customer in real-time that the provider accepted the offer
            await _hubContext.Clients.User(booking.CustomerId.ToString())
                .SendAsync("BookingStatusUpdated", response.Data);
            await _hubContext.Clients.Group(booking.CustomerId.ToString())
                .SendAsync("BookingStatusUpdated", response.Data);

            // Notify the dry cleaner that their offer was accepted
            await _hubContext.Clients.User(dryCleanerUserId.ToString())
                .SendAsync("BookingStatusUpdated", response.Data);
            await _hubContext.Clients.Group(dryCleanerUserId.ToString())
                .SendAsync("BookingStatusUpdated", response.Data);

            return response;
        }

        public async Task<Response<DryCleaningDto>> RaisePrice(Guid bookingId, decimal newPrice)
        {
            var response = new Response<DryCleaningDto>();
            var booking = await _unitOfWork.DryCleaning.GetByIdWithDetailsAsync(bookingId);
            if (booking == null)
            {
                response.Success = false;
                response.Message = "Booking not found.";
                return response;
            }
            if (newPrice <= booking.EstimateTotalAMount)
            {
                response.Success = false;
                response.Message = "You can only increase the price.";
                return response;
            }
            booking.EstimateTotalAMount = newPrice;
            await _unitOfWork.DryCleaning.UpdateAsync(booking);
            response.Success = true;
            response.Message = "Price raised successfully.";
            var data = new DryCleaningDto
            {
                DrycleaningId = booking.Id,
                CustomerId = booking.CustomerId,
                DryCleanerId = booking.DryCleanerId,
                PackageId = booking.PackageId,
                DeliveryDate = booking.DeliveryDate,
                Status = booking.Status.ToString(),
                TotalAmount = booking.TotalAmount,
                EstimateTotalAmount = booking.EstimateTotalAMount,
                DryCleaningItems = booking.DryCleaningItems.Select(x => new DryCleaningItemDto
                {
                    Id = x.Id,
                    ItemId = x.LaundryItemId ?? x.DryCleanerLaundryItemId,
                    ItemName = x.LaundryItem?.Name ?? x.DryCleanerLaundryItem?.LaundryItem?.Name,
                    Quantity = x.Quantity,
                    TotalPrice = x.TotalPrice
                }).ToList()
            };
            response.Data = data;
            var notify = await NotifyServiceProviders(response.Data);
            data.ProvidersNotified = notify.ProvidersNotified;
            return response;
        }
    }

}
