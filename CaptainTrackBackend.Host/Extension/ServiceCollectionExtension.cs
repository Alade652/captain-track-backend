using System.Text;
using CaptainTrackBackend.Application.Abstraction.Interface.Maps;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.Ambulance;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.Carwash;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.Courier;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.GasDelivery;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.House_Moving;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.HouseCleaning;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.RideHailing;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.VehicleTowing;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.WaterSupply;
using CaptainTrackBackend.Application.Abstraction.Interface.Services;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.Ambulance;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.CarWash;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.Courier;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.GasDelivery;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.House_Moving;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.HouseCleaning;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.RideHailing;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.VehicleTowing;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.WaterSupply;
using CaptainTrackBackend.Application.Authentcication;
using CaptainTrackBackend.Application.AuthService;
using CaptainTrackBackend.Application.Services;
using CaptainTrackBackend.Application.Services.FileUpload;
using CaptainTrackBackend.Application.Services.ServiceProviders;
using CaptainTrackBackend.Application.Services.ServiceProviders.Ambulance;
using CaptainTrackBackend.Application.Services.ServiceProviders.CarWash;
using CaptainTrackBackend.Application.Services.ServiceProviders.Courier;
using CaptainTrackBackend.Application.Services.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Application.Services.ServiceProviders.GasDelivery;
using CaptainTrackBackend.Application.Services.ServiceProviders.House_Moving;
using CaptainTrackBackend.Application.Services.ServiceProviders.HouseCleaning;
using CaptainTrackBackend.Application.Services.ServiceProviders.RideHailing;
using CaptainTrackBackend.Application.Services.ServiceProviders.VehicleTowing;
using CaptainTrackBackend.Application.Services.ServiceProviders.WaterSupply;
using CaptainTrackBackend.Persistence.Map;
using CaptainTrackBackend.Persistence.PaymentIntegration;
using CaptainTrackBackend.Persistence.Repositories;
using CaptainTrackBackend.Persistence.Repositories.ServiceProviders;
using CaptainTrackBackend.Persistence.Repositories.ServiceProviders.Ambulance;
using CaptainTrackBackend.Persistence.Repositories.ServiceProviders.CarWash;
using CaptainTrackBackend.Persistence.Repositories.ServiceProviders.Courier;
using CaptainTrackBackend.Persistence.Repositories.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Persistence.Repositories.ServiceProviders.GasDelivery;
using CaptainTrackBackend.Persistence.Repositories.ServiceProviders.House_Moving;
using CaptainTrackBackend.Persistence.Repositories.ServiceProviders.HouseCleaning;
using CaptainTrackBackend.Persistence.Repositories.ServiceProviders.RideHailing;
using CaptainTrackBackend.Persistence.Repositories.ServiceProviders.VehicleTowing;
using CaptainTrackBackend.Persistence.Repositories.ServiceProviders.WaterSupply;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CaptainTrackBackend.Host.Extension
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                .AddScoped<ITokenBlacklistRepository, TokenBlacklistRepository>()
                .AddScoped<IUnitofWork,UnitofWork>()
                .AddScoped<IPaymentRepository, PaymentRepository>()
                .AddScoped<IRatingRepository, RatingRepository>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<ICustomerRepository, CustomerRepository>()
                .AddScoped<IDriversRepository, DriversRepository>()
                .AddScoped<IAdminRepository, AdminRepository>()
                .AddScoped<ITransactionRepository, TransactionRepository>()
                .AddScoped<ITripRepository, TripRepository>()
                .AddScoped<IDryCleanerRepository, DryCleanerRepository>()
                .AddScoped<IDryCleaningRepository, DryCleaningRepository>()
                .AddScoped<ILaundryItemRepository, LaundryItemRepository>()
                .AddScoped<ILaundryPackageRepository, LaundryPackageRepository>()
                .AddScoped<IGasSupplierRepo, GasSupplierRepo>()
                .AddScoped<IGasDeliveryRepo, GasDeliveryRepo>()
                .AddScoped<IHouseCleanerRepo, HouseCleanerRepo>()
                .AddScoped<IHouseCleaningRepo, HouseCleaningRepo>()
                .AddScoped<ITruckOperatorRepo, TruckOperatorRepo>()
                .AddScoped<ITowingRepo, TowingRepo>()
                .AddScoped<IAmbulanceRepo, AmbulanceRepo>()
                .AddScoped<IAmbulanceBookingRepo, AmbulanceBookingRepo>()
                .AddScoped<IHouseMovingRepo, HouseMovingRepo>()
                .AddScoped<IHouseMoverRepo, HouseMoverRepo>()
                .AddScoped<ICarWasherRepo, CarWasherRepo>()
                .AddScoped<ICarWashingRepo, CarWashingRepo>()
                .AddScoped<IRiderorParkRepo,RiderorParkRepo>()
                .AddScoped<ICourierRepo, CourierRepo>()
                .AddScoped<IWaterSupplierRepo, WaterSupplierRepo>()
                .AddScoped<IWaterSupplingRepo, WaterSupplingRepo>();
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services
                .AddScoped<IAuthService, AuthService>()
                .AddScoped<IMapServices, MapServices>()
                .AddScoped<ITrackingHub, TrackingHub>()
                .AddScoped<IOTPService, OTPService>()
                .AddScoped<IFileUploadService, FileUploadService>()
                .AddScoped<IFlutterwaveService, FlutterwaveService>()
                .AddScoped<IRatingService, RatingService>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<IEmailService, EmailService>()
                .AddScoped<IAdminServices, AdminService>()
                .AddScoped<IDriversServices, DriversService>()
                .AddScoped<ICustomerService, CustomerService>()
                .AddScoped<ITripService, TripService>()
                .AddScoped<IDryCleanerService, DryCleanerService>()
                .AddScoped<IDryCleaningService, DryCleaningService>()
                .AddScoped<ILaundryPackageService, LaundryPackageService>()
                .AddScoped<ILaundryItemService, LaundryItemService>()
                .AddScoped<IGasSupplierService, GasSupplierService>()
                .AddScoped<IGasDeliveryService, GasDeliveryService>()
                .AddScoped<IHouseCleanerService, HouseCleanerService>()
                .AddScoped<IHouseCleaningService, HouseCleaningService>()
                .AddScoped<ITruckOperatorService, TruckOperatorService>()
                .AddScoped<ITowingService, TowingService>()
                .AddScoped<IAmbulanceSerivice, AmbulanceService>()
                .AddScoped<IAmbulanceBookingService, AmbulanceBookingService>()
                .AddScoped<IHouseMoverService, HouseMoverService>()
                .AddScoped<IHouseMovingService, HouseMovingService>()
                .AddScoped<ICarWasherService, CarWasherService>()
                .AddScoped<ICarWashingService, CarWashingService>()
                .AddScoped<IRiderorParkService, RiderorParkService>()
                .AddScoped<ICourierService, CourierService>()
                .AddScoped<IWaterSupplierService, WaterSupplierService>()
                .AddScoped<IWaterSupplingService, WaterSupplingService>();
        }

        public static IServiceCollection RegisterJWT(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });
            return services;
        }


    }
}
