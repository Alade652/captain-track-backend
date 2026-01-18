using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.House_Moving
{
    public class HouseMoving : AuditableEntity
    {
        public string PickupLocation { get; set; }
        public string DropoffLocation { get; set; }
        public DateTime? MovingDate { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public string Distance { get; set; }
        public string Duration { get; set; }
        public string? DistanceToPickup { get; set; }
        public string? DurationToPickup { get; set; }
        public decimal EstimatedFare { get; set; }
        public decimal Price { get; set; }
        public decimal NegotiatedPrice { get; set; }
        public ServiceStatus Status { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? HouseMoverId { get; set; }
        public Guid PackageId { get; set; }
        public Guid TruckId { get; set; }
        public Customer Customer { get; set; }
        public HouseMover HouseMover { get; set; }
        public HouseMovingPackage Package { get; set; }
        public HouseMovingTruck Truck { get; set; }

    }
}
