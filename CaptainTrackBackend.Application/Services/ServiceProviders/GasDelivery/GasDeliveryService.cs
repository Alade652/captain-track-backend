using CaptainTrackBackend.Application.Abstraction.Interface.Maps;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.GasDelivery;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.CarWash;
using CaptainTrackBackend.Application.DTO.ServiceProviders.GasDelivery;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.GasDelivery;
using CaptainTrackBackend.Domain.Enum;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CaptainTrackBackend.Application.Services.ServiceProviders.GasDelivery
{
    public class GasDeliveryService : IGasDeliveryService
    {
        private readonly IUnitofWork _unitOfWork;
        private readonly IMapServices _mapServices;
        private readonly IHubContext<NegotiationHub> _hubContext;
        private readonly ILogger<GasDeliveryService> _logger;

        public GasDeliveryService(IUnitofWork unitOfWork, IMapServices mapServices,
            IHubContext<NegotiationHub> hubContext, ILogger<GasDeliveryService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapServices = mapServices;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task<NotificationResult> NotifyServiceProviders(GasDeliveryDto booking)
        {
            var response = new NotificationResult();
            try
            {
                var providers = await _unitOfWork.GasSupplier.GetAllByExpression(x => x.ServiceProviderRole == ServiceProviderRole.Freelancer && x.IsAvailable == true);
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
                            booking.DurationtoLocation = element.duration.text;


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
                _logger.LogError(ex, "Error in NotifyServiceProvidersAsync for booking ID {BookingId}", booking.GasSupplierId);
            }

            return response;
        }


        public async Task<Response<GasDeliveryDto>> Book(Guid gasDeliveryId)
        {
            var response = new Response<GasDeliveryDto>();
            var gasDelivery = await _unitOfWork.GasDelivery.GetAsync(x => x.Id == gasDeliveryId);
            if (gasDelivery == null)
            {
                response.Success = false;
                response.Message = "Gas delivery not found.";
                return response;
            }
            if (gasDelivery.GasSupplierId != null)
            {
                gasDelivery.Status = ServiceStatus.Booked;
                gasDelivery.TotalAmount = gasDelivery.EstimateTotalAmount;
                await _unitOfWork.GasDelivery.UpdateAsync(gasDelivery);
                response.Success = true;
                response.Message = "Gas delivery booking successfully.";
                response.Data = new GasDeliveryDto
                {
                    Id = gasDelivery.Id,
                    CustomerId = gasDelivery.CustomerId,
                    CustomerName = gasDelivery.Customer.FirstName + " " + gasDelivery.Customer.LastName,
                    GasSupplierId = gasDelivery.GasSupplierId,
                    DeliveryAddress = gasDelivery.DeliveryAddress,
                    DeliveryDate = gasDelivery.DeliveryDate,
                    CylinderCount = gasDelivery.CylinderCount,
                    EstimateTotalAmount = gasDelivery.EstimateTotalAmount,
                    TotalAmount = gasDelivery.TotalAmount,
                    DeliveryNote = gasDelivery.DeliveryNote,
                    ServiceMethod = gasDelivery.ServiceMethod.ToString(),
                    Status = gasDelivery.Status.ToString(),
                    Cylinders = gasDelivery.Cylinders.Select(c => new CylinderDto
                    {
                        CylinderId = c.Id,
                        CylinderType = c.CylinderType,
                        TotalPrice = c.Price,
                        Quantity = c.AmountOfGas
                    }).ToList()
                };
                return response;
            }
            else
            {
                gasDelivery.Status = ServiceStatus.Pending;
                await _unitOfWork.GasDelivery.UpdateAsync(gasDelivery);
                response.Success = true;
                response.Message = "Gas delivery booking is pending for supplier selection.";
                var data = new GasDeliveryDto
                {
                    Id = gasDelivery.Id,
                    CustomerId = gasDelivery.CustomerId,
                    CustomerName = gasDelivery.Customer.FirstName + " " + gasDelivery.Customer.LastName,
                    GasSupplierId = gasDelivery.GasSupplierId,
                    DeliveryAddress = gasDelivery.DeliveryAddress,
                    DeliveryDate = gasDelivery.DeliveryDate,
                    CylinderCount = gasDelivery.CylinderCount,
                    TotalAmount = gasDelivery.TotalAmount,
                    DeliveryNote = gasDelivery.DeliveryNote,
                    ServiceMethod = gasDelivery.ServiceMethod.ToString(),
                    Status = gasDelivery.Status.ToString(),
                    Cylinders = gasDelivery.Cylinders.Select(c => new CylinderDto
                    {
                        CylinderId = c.Id,
                        CylinderType = c.CylinderType,
                        TotalPrice = c.Price,
                        Quantity = c.AmountOfGas
                    }).ToList()
                };
                var notificationResult = await NotifyServiceProviders(data);
                data.ProvidersNotified = notificationResult.ProvidersNotified;
                response.Data = data;
                return response;

            }

        }

        public async Task<Response<GasDeliveryDto>> InitBooking(Guid customerId, Guid gasSupplierUserId, GasDeliveryRequestDto request)
        {
            var gasSupplier = await _unitOfWork.GasSupplier.GetAsync(g => g.UserId == gasSupplierUserId);
            if (gasSupplier == null)
            {
                return new Response<GasDeliveryDto>
                {
                    Success = false,
                    Message = "Gas supplier not found."
                };
            }

            var gasDelivery = new GasDelivering
            {
                CustomerId = customerId,
                GasSupplierId = gasSupplier.Id,
                DeliveryAddress = request.DeliveryAddress,
                DeliveryDate = request.DeliveryDate,
                CylinderCount = 0,
                EstimateTotalAmount = 0,
                Status = ServiceStatus.Pending,
                ServiceMethod = request.ServiceMethod,
                DeliveryNote = request.DeliveryNote,
                Cylinders = new List<Cylinder>(),
            };
            foreach (var item in request.Cylinders)
            {
                var cylinder = new Cylinder
                {
                    GasDeliveryId = gasDelivery.Id,
                    CylinderType = item.CylinderType,
                    AmountOfGas = item.Quantity,
                    Price = gasSupplier.PricePerKg * item.Quantity
                };
                gasDelivery.CylinderCount++;
                gasDelivery.EstimateTotalAmount += cylinder.Price;
                gasDelivery.Cylinders.Add(cylinder);
            }
            await _unitOfWork.GasDelivery.AddAsync(gasDelivery);

            return new Response<GasDeliveryDto>
            {
                Success = true,
                Data = new GasDeliveryDto
                {
                    Id = gasDelivery.Id,
                    CustomerId = gasDelivery.CustomerId,
                    GasSupplierId = gasDelivery.GasSupplierId,
                    DeliveryAddress = gasDelivery.DeliveryAddress,
                    DeliveryDate = gasDelivery.DeliveryDate,
                    CylinderCount = gasDelivery.CylinderCount,
                    EstimateTotalAmount = gasDelivery.EstimateTotalAmount,
                    DeliveryNote = gasDelivery.DeliveryNote,
                    ServiceMethod = gasDelivery.ServiceMethod.ToString(),
                    Cylinders = gasDelivery.Cylinders.Select(c => new CylinderDto
                    {
                        CylinderId = c.Id,
                        CylinderType = c.CylinderType,
                        TotalPrice = c.Price,
                        Quantity = c.AmountOfGas
                    }).ToList()
                },
                Message = "Booking initialized successfully."
            };
        }

        public async Task<Response<GasDeliveryDto>> InitBooking(Guid customerId, GasDeliveryRequestDto request)
        {
            var pricePerKg = await _unitOfWork.Context.Prices.FirstOrDefaultAsync();
            var gasDelivery = new GasDelivering
            {
                CustomerId = customerId,
                DeliveryAddress = request.DeliveryAddress,
                DeliveryDate = request.DeliveryDate,
                CylinderCount = 0,
                EstimateTotalAmount = 0,
                Status = ServiceStatus.Pending,
                ServiceMethod = request.ServiceMethod,
                DeliveryNote = request.DeliveryNote,
                Cylinders = new List<Cylinder>(),
            };
            foreach (var item in request.Cylinders)
            {
                var cylinder = new Cylinder
                {
                    GasDeliveryId = gasDelivery.Id,
                    CylinderType = item.CylinderType,
                    AmountOfGas = item.Quantity,
                    Price = pricePerKg.GasPriceperKg * item.Quantity
                };
                gasDelivery.CylinderCount++;
                gasDelivery.EstimateTotalAmount += cylinder.Price;
                gasDelivery.Cylinders.Add(cylinder);
            }
            await _unitOfWork.GasDelivery.AddAsync(gasDelivery);

            return new Response<GasDeliveryDto>
            {
                Success = true,
                Data = new GasDeliveryDto
                {
                    Id = gasDelivery.Id,
                    CustomerId = gasDelivery.CustomerId,
                    GasSupplierId = gasDelivery.GasSupplierId,
                    DeliveryAddress = gasDelivery.DeliveryAddress,
                    DeliveryDate = gasDelivery.DeliveryDate,
                    CylinderCount = gasDelivery.CylinderCount,
                    EstimateTotalAmount = gasDelivery.EstimateTotalAmount,
                    DeliveryNote = gasDelivery.DeliveryNote,
                    ServiceMethod = gasDelivery.ServiceMethod.ToString(),
                    Cylinders = gasDelivery.Cylinders.Select(c => new CylinderDto
                    {
                        CylinderId = c.Id,
                        CylinderType = c.CylinderType,
                        TotalPrice = c.Price,
                        Quantity = c.AmountOfGas
                    }).ToList()
                },
                Message = "Booking initialized successfully."
            };
        }

       
        public async Task<Response<List<GasDeliveryDto>>> GetPending(string location)
        {
            var response = new Response<List<GasDeliveryDto>>();
            var gasDelivery = await _unitOfWork.GasDelivery.GetAllByExpressionAsync(x => x.GasSupplierId == null && x.Status == ServiceStatus.Pending);
            if (gasDelivery == null || !gasDelivery.Any())
            {
                response.Success = false;
                response.Message = "No pending gas deliveries found.";
                return response;
            }

            var bookings = new List<GasDeliveryDto>();
            foreach (var x in gasDelivery)
            {
                var distandDura = await _mapServices.GetDistanceAndDurationAsync(location, x.DeliveryAddress);

                var result = await _mapServices.GetDistanceAsync(location, x.DeliveryAddress);
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
                    bookings.Add(new GasDeliveryDto
                    {
                        Id = x.Id,
                        CustomerId = x.CustomerId,
                        CustomerName = x.Customer.FirstName + " " + x.Customer.LastName,
                        GasSupplierId = x.GasSupplierId,
                        DeliveryAddress = x.DeliveryAddress,
                        DeliveryDate = x.DeliveryDate,
                        CylinderCount = x.CylinderCount,
                        EstimateTotalAmount = x.EstimateTotalAmount,
                        TotalAmount = x.TotalAmount,
                        DeliveryNote = x.DeliveryNote,
                        ServiceMethod = x.ServiceMethod.ToString(),
                        Status = x.Status.ToString(),
                        DistancetoLocation = distance,
                        DurationtoLocation = duration,
                        Cylinders = x.Cylinders.Select(c => new CylinderDto
                        {
                            CylinderId = c.Id,
                            CylinderType = c.CylinderType,
                            TotalPrice = c.Price,
                            Quantity = c.AmountOfGas
                        }).ToList()
                    });
                }
            }

            response.Success = true;
            response.Message = "Pending gas deliveries retrieved successfully.";
            response.Data = bookings;
            return response;
        }

        public async Task<Response<List<GasDeliveryDto>>> GetBookings(Guid userId)
        {
/*            var gasSupplier = await _unitOfWork.GasSupplier.GetAsync(x => x.UserId == supplierUserId);
            if (gasSupplier == null)
            {
                return new Response<List<GasDeliveryDto>>
                {
                    Success = false,
                    Message = "Gas supplier not found."
                };
            }*/
            var gasDeliveries = await _unitOfWork.GasDelivery.GetAllByExpressionAsync(x => x.GasSupplierId == userId || x.CustomerId == userId && x.Status != ServiceStatus.Init);
            if (gasDeliveries == null || !gasDeliveries.Any())
            {
                return new Response<List<GasDeliveryDto>>
                {
                    Success = false,
                    Message = "No bookings found for this gas supplier."
                };
            }
            var gasDeliveryDtos = gasDeliveries.Select(x => new GasDeliveryDto
            {
                Id = x.Id,
                CustomerId = x.CustomerId,
                CustomerName = x.Customer.FirstName + " " + x.Customer.LastName,
                GasSupplierId = x.GasSupplierId,
                DeliveryAddress = x.DeliveryAddress,
                DeliveryDate = x.DeliveryDate,
                CylinderCount = x.CylinderCount,
                EstimateTotalAmount = x.EstimateTotalAmount,
                TotalAmount = x.TotalAmount,
                DeliveryNote = x.DeliveryNote,
                ServiceMethod = x.ServiceMethod.ToString(),
                Status = x.Status.ToString(),
                Cylinders = x.Cylinders.Select(c => new CylinderDto
                {
                    CylinderId = c.Id,
                    CylinderType = c.CylinderType,
                    TotalPrice = c.Price,
                    Quantity = c.AmountOfGas
                }).ToList()
            }).ToList();
            return new Response<List<GasDeliveryDto>>
            {
                Success = true,
                Message = "Gas supplier bookings retrieved successfully.",
                Data = gasDeliveryDtos
            };
        }

        public async Task<Response<bool>> AcceptBooking(Guid gasDeliveryId)
        {
            var response = new Response<bool>();
            var gasDelivery = await _unitOfWork.GasDelivery.GetAsync(x => x.Id == gasDeliveryId);
            if (gasDelivery == null)
            {
                response.Success = false;
                response.Message = "Gas delivery not found.";
                return response;
            }
            gasDelivery.Status = ServiceStatus.Booked;
            await _unitOfWork.GasDelivery.UpdateAsync(gasDelivery);
            response.Success = true;
            response.Message = "Gas delivery booking accepted successfully.";
            return response;
        }

        public async Task<Response<bool>> RejectBooking(Guid gasDeliveryId)
        {
            var response = new Response<bool>();
            var booking = await _unitOfWork.GasDelivery.GetAsync(x => x.Id == gasDeliveryId);
            if (booking == null)
            {
                response.Success = false;
                response.Message = "booking not found";
                return response;
            }

            booking.Status = ServiceStatus.Rejected;
            await _unitOfWork.GasDelivery.UpdateAsync(booking);
            response.Success = true;
            response.Message = "Booking Rejected";
            return response;
        }

        public async Task<Response<bool>> CancelBooking(Guid gasDeliveryId)
        {
            var response = new Response<bool>();
            var booking = await _unitOfWork.GasDelivery.GetAsync(x => x.Id == gasDeliveryId);
            if (booking == null)
            {
                response.Success = false;
                response.Message = "booking not found";
                return response;
            }

            booking.Status = ServiceStatus.Cancelled;
            await _unitOfWork.GasDelivery.UpdateAsync(booking);
            response.Success = true;
            response.Message = "Booking Cancelled";
            return response;
        }

        public async Task<Response<GasDeliveryDto>> AcceptOffer(Guid bookingId, Guid gasSupplierUserId, decimal offerAmount)
        {
            var response = new Response<GasDeliveryDto>();
            var gasDelivery = await _unitOfWork.GasDelivery.GetAsync(x => x.Id == bookingId);
            if (gasDelivery == null)
            {
                response.Success = false;
                response.Message = "Booking not found.";
                return response;
            }
            var gasSupplier = await _unitOfWork.GasSupplier.GetAsync(x => x.UserId == gasSupplierUserId);
            if (gasSupplier == null)
            {
                response.Success = false;
                response.Message = "Gas supplier not found.";
                return response;
            }

            gasDelivery.GasSupplierId = gasSupplier.Id;
            gasDelivery.TotalAmount = offerAmount;
            gasDelivery.Status = ServiceStatus.Booked;
            await _unitOfWork.GasDelivery.UpdateAsync(gasDelivery);
            response.Success = true;
            response.Message = "Offer accepted successfully.";
            response.Data = new GasDeliveryDto
            {
                Id = gasDelivery.Id,
                CustomerId = gasDelivery.CustomerId,
                CustomerName = gasDelivery.Customer.FirstName + " " + gasDelivery.Customer.LastName,
                GasSupplierId = gasDelivery.GasSupplierId,
                DeliveryAddress = gasDelivery.DeliveryAddress,
                DeliveryDate = gasDelivery.DeliveryDate,
                CylinderCount = gasDelivery.CylinderCount,
                EstimateTotalAmount = gasDelivery.EstimateTotalAmount,
                TotalAmount = gasDelivery.TotalAmount,
                DeliveryNote = gasDelivery.DeliveryNote,
                ServiceMethod = gasDelivery.ServiceMethod.ToString(),
                Status = gasDelivery.Status.ToString(),
                Cylinders = gasDelivery.Cylinders.Select(c => new CylinderDto
                {
                    CylinderId = c.Id,
                    CylinderType = c.CylinderType,
                    TotalPrice = c.Price,
                    Quantity = c.AmountOfGas
                }).ToList()
            };
            return response;
        }

        public async Task<Response<GasDeliveryDto>> RaisePrice(Guid bookingId, decimal newPrice)
        {
            var response = new Response<GasDeliveryDto>();
            var gasDelivery = await _unitOfWork.GasDelivery.GetAsync(x => x.Id == bookingId);
            if (gasDelivery == null)
            {
                response.Success = false;
                response.Message = "Booking not found.";
                return response;
            }

            if (newPrice <= gasDelivery.EstimateTotalAmount)
            {
                response.Success = false;
                response.Message = "You can only increase the price.";
                return response;
            }
            gasDelivery.EstimateTotalAmount = newPrice;
            await _unitOfWork.GasDelivery.UpdateAsync(gasDelivery);
            response.Success = true;
            response.Message = "Price raised successfully.";
            var data = new GasDeliveryDto
            {
                Id = gasDelivery.Id,
                CustomerId = gasDelivery.CustomerId,
                CustomerName = gasDelivery.Customer.FirstName + " " + gasDelivery.Customer.LastName,
                GasSupplierId = gasDelivery.GasSupplierId,
                DeliveryAddress = gasDelivery.DeliveryAddress,
                DeliveryDate = gasDelivery.DeliveryDate,
                CylinderCount = gasDelivery.CylinderCount,
                EstimateTotalAmount = gasDelivery.EstimateTotalAmount,
                TotalAmount = gasDelivery.TotalAmount,
                DeliveryNote = gasDelivery.DeliveryNote,
                ServiceMethod = gasDelivery.ServiceMethod.ToString(),
                Status = gasDelivery.Status.ToString(),
                Cylinders = gasDelivery.Cylinders.Select(c => new CylinderDto
                {
                    CylinderId = c.Id,
                    CylinderType = c.CylinderType,
                    TotalPrice = c.Price,
                    Quantity = c.AmountOfGas
                }).ToList()
            };
            var notify = await NotifyServiceProviders(response.Data);
            data.ProvidersNotified = notify.ProvidersNotified;
            response.Data = data;
            return response;
        }
    }
}
