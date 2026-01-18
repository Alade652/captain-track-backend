using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.RideHailing;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptainTrackBackend.Application.Services.FileUpload;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Application.Abstraction.Interface.Services;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.GasDelivery;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.HouseCleaning;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.VehicleTowing;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.Ambulance;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.House_Moving;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.Carwash;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.Courier;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.WaterSupply;

namespace CaptainTrackBackend.Persistence.Repositories
{
    public class UnitofWork : IUnitofWork
    {
        public IEmailService Email { get; }
        public ICustomerRepository Customer { get; }
        public IUserRepository User { get; }
        public IAdminRepository Admin { get; }
        public IDriversRepository Driver { get; }
        public IDryCleanerRepository DryCleaner { get; }
        public IDryCleaningRepository DryCleaning { get; }
        public ITransactionRepository Transaction { get; }
        public IPaymentRepository Payment { get; }
        public IRatingRepository Rating { get; }
        public ITripRepository Trip { get; }
        public ILaundryItemRepository LaundryItem { get; }
        public ILaundryPackageRepository LaundryPackage { get; }
        public IGasSupplierRepo GasSupplier { get; }
        public IGasDeliveryRepo GasDelivery { get; }
        public IHouseCleanerRepo HouseCleaner { get; }
        public IHouseCleaningRepo HouseCleaning { get; }
        public ITruckOperatorRepo TruckOperator { get; }
        public ITowingRepo Towing { get; }
        public IAmbulanceRepo Ambulance { get; }
        public IAmbulanceBookingRepo AmbulanceBooking { get; }
        public IHouseMovingRepo HouseMoving { get; }
        public IHouseMoverRepo HouseMover { get; }
        public ICarWasherRepo CarWasher { get; }
        public ICarWashingRepo CarWashing { get; }
        public IRiderorParkRepo RiderorPark { get; }
        public ICourierRepo Courier { get; }
        public IWaterSupplierRepo WaterSupplier { get; }
        public IWaterSupplingRepo WaterSuppling { get; }

        public IFileUploadService FileUpload { get; }
        public ApplicationDbContext Context { get; }

        public UnitofWork(ApplicationDbContext context, 
                          IEmailService emailService,
                          ICustomerRepository customerRepository,
                          IUserRepository userRepository,
                          IAdminRepository adminRepository,
                          IDriversRepository driversRepository,
                          IDryCleanerRepository dryCleanerRepository,
                          IDryCleaningRepository dryCleaningRepository,
                          ITransactionRepository transactionRepository,
                          IPaymentRepository paymentRepository,
                          IRatingRepository ratingRepository,
                          ITripRepository tripRepository,
                          ILaundryItemRepository LaundrytemRepository,
                          ILaundryPackageRepository laundryPackageRepository,
                          IGasSupplierRepo gasSupplierRepo,
                          IGasDeliveryRepo gasDeliveryRepo,
                          IHouseCleanerRepo houseCleanerRepo,
                          IHouseCleaningRepo houseCleaningRepo,
                          ITruckOperatorRepo truckOperatorRepo,
                          ITowingRepo towingRepo,
                          IAmbulanceRepo ambulanceRepo,
                          IAmbulanceBookingRepo ambulanceBookingRepo,
                          IHouseMoverRepo houseMoverRepo,
                          IHouseMovingRepo houseMovingRepo,
                          ICarWashingRepo carWashingRepo,
                          ICarWasherRepo carWasherRepo,
                          IRiderorParkRepo riderorParkRepo,
                          ICourierRepo courier,
                          IWaterSupplingRepo waterSuppling,
                          IWaterSupplierRepo waterSupplierRepo,


                          IFileUploadService fileUploadService)
        {
            Context = context;
            Email = emailService;
            Customer = customerRepository;
            User = userRepository;
            Admin = adminRepository;
            Driver = driversRepository;
            DryCleaner = dryCleanerRepository;
            DryCleaning = dryCleaningRepository;
            Transaction = transactionRepository;
            Payment = paymentRepository;
            Rating = ratingRepository;
            Trip = tripRepository;
            LaundryItem = LaundrytemRepository;
            LaundryPackage = laundryPackageRepository;
            GasSupplier = gasSupplierRepo;
            GasDelivery = gasDeliveryRepo;
            HouseCleaner = houseCleanerRepo;
            HouseCleaning = houseCleaningRepo;
            TruckOperator = truckOperatorRepo;
            Towing = towingRepo;
            Ambulance = ambulanceRepo;
            AmbulanceBooking = ambulanceBookingRepo;
            HouseMoving = houseMovingRepo;
            HouseMover = houseMoverRepo;
            CarWashing = carWashingRepo;
            CarWasher = carWasherRepo;
            RiderorPark = riderorParkRepo;
            Courier = courier;
            WaterSuppling = waterSuppling;
            WaterSupplier = waterSupplierRepo;

            FileUpload = fileUploadService;

        }
    }
}
