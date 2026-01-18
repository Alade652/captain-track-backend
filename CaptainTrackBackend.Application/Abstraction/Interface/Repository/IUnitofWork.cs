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
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Application.Services.FileUpload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Repository
{
    public interface IUnitofWork
    {
        IEmailService Email { get; }
        ICustomerRepository Customer { get; }
        IUserRepository User { get; }
        IAdminRepository Admin { get; }
        IDriversRepository Driver { get; }
        IDryCleanerRepository DryCleaner { get; }
        IDryCleaningRepository DryCleaning { get; }
        ITransactionRepository Transaction { get; }
        IPaymentRepository Payment { get; }
        IRatingRepository Rating { get; }
        ITripRepository Trip { get; }
        ILaundryItemRepository LaundryItem { get; }
        ILaundryPackageRepository LaundryPackage { get; }
        IGasSupplierRepo GasSupplier { get; }
        IGasDeliveryRepo GasDelivery { get; }
        IHouseCleanerRepo HouseCleaner { get; }
        IHouseCleaningRepo HouseCleaning { get; }
        ITruckOperatorRepo TruckOperator { get; }
        ITowingRepo Towing { get; }
        IAmbulanceRepo Ambulance { get; }
        IAmbulanceBookingRepo AmbulanceBooking { get; }
        IHouseMovingRepo HouseMoving { get; }
        IHouseMoverRepo HouseMover { get; }
        ICarWasherRepo CarWasher { get; }
        ICarWashingRepo CarWashing { get; }
        IRiderorParkRepo RiderorPark { get; }
        ICourierRepo Courier { get; }
        IWaterSupplierRepo WaterSupplier { get; }
        IWaterSupplingRepo WaterSuppling { get; }


        IFileUploadService FileUpload { get; }
        ApplicationDbContext Context { get; }

    }
}
