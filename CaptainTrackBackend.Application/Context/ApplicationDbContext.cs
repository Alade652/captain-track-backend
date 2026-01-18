using CaptainTrackBackend.Application.Authentcication;
using CaptainTrackBackend.Domain.Entities;
using CaptainTrackBackend.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.RideHailing;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.GasDelivery;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.HouseCleaning;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.VehicleTowing;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.Ambulance;
using CaptainTrackBackend.Domain.Entities.ServiceProviders;
using CaptainTrackBackend.Application.Services.ServiceProviders.RideHailing;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.House_Moving;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.CarWash;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.Courier;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.WaterSupply;

namespace CaptainTrackBackend.Application.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSet properties for your entities
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<ServiceProviding> ServiceProvidings { get; set; }
        public DbSet<UserServiceProviding> UserServiceProvidings { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<TripNegotiation> TripNegotiations { get; set; }
        public DbSet<TokenBlacklist> TokenBlacklists { get; set; }
        public DbSet<DryCleaner> DryCleaners { get; set; }
        public DbSet<LaundryItem> LaundryItems { get; set; }
        public DbSet<LaundryPackage> LaundryPackages { get; set; }
        public DbSet<DryCleanerLaundryItem> DryCleanerLaundryItems { get; set; }
        public DbSet<DryCleaningItem> DryCleaningItems { get; set; }
        public DbSet<DryClean> DryCleanings { get; set; }
        public DbSet<DryCleaningNegotiation> DryCleaningNegotiation { get; set; }
        //public DbSet<PricePerKg> PricePerKgs { get; set; }
        public DbSet<GasSupplier> GasSuppliers { get; set; }
        public DbSet<GasDelivering> GasDeliveries { get; set; }
        public DbSet<Cylinder> Cylinders { get; set; }
        public DbSet<GasDeliveryNegotiation> GasDeliveryNegotiations { get; set; }
        public DbSet<HouseCleaner> HouseCleaners { get; set; }
        public DbSet<Housecleaning> HouseCleanings { get; set; }
        public DbSet<HouseCleanerItem> HouseCleanerItems { get; set; }
        public DbSet<HouseCleaningItem> HouseCleaningItems { get; set; }
        public DbSet<HouseCleanerPackage> HouseCleanerPackages { get; set; }
        public DbSet<HouseCleaningNegotiation> HouseCleaningNegotiations { get; set; }
        public DbSet<TowTruckOperator> TowTruckOperators { get; set; }
        public DbSet<Truck> Trucks { get; set; }
        public DbSet<Towing> Towings { get; set; }
        public DbSet<TowingNegotiation> TowingNegotiations { get; set; }
        public DbSet<AmbulanceCompany> Ambulances { get; set; }
        public DbSet<AmbulanceBooking> AmbulanceBookings { get; set; }
        public DbSet<AmbulanceNegotiation> AmbulanceNegotiations { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<HouseMoving> HouseMovings { get; set; }
        public DbSet<HouseMover> HouseMover { get; set; }
        public DbSet<HouseMovingPackage> HouseMovingPackages { get; set; }
        public DbSet<HouseMovingTruck> HouseMovingTrucks { get; set; }
        public DbSet<HouseMovingNegotiation> HouseMovingNegotiations { get; set; }
        public DbSet<CarWasher> CarWashers { get; set; }
        public DbSet<CarWashing> CarWashings { get; set; }
        public DbSet<CarWashingitem> CarWashingItems { get; set; }
        public DbSet<CarWashItem> CarWashItems { get; set; }
        public DbSet<CarWashNegotiation> CarWashNegotiations { get; set; }
        public DbSet<RiderorPark> RiderorParks { get; set; }
        public DbSet<CourierVehicle> CourierVehicles { get; set; }
        public DbSet<Courier_Service> Courier_Services { get; set; }
        public DbSet<CourierNegotiation> CourierNegotiations { get; set; }
        public DbSet<WaterSupplier> WaterSuppliers { get; set; }
        public DbSet<WaterSuppling> WaterSupplings { get; set; }


    }
}
