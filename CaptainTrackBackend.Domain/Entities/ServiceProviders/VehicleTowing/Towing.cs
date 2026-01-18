using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.VehicleTowing
{
    public class Towing : AuditableEntity
    {
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public Guid? TowTruckOperatorId { get; set; }
        public TowTruckOperator TowTruckOperator { get; set; }
        public Guid TruckId { get; set; }
        public Truck Truck { get; set; }
        public string TruckName { get; set; }
        public string PickupLocation { get; set; }
        public string DropOffLocation { get; set; }
        public string CarModel { get; set; }
        public decimal EstimatedPrice { get; set; }
        public decimal NegotiatedPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Distance { get; set; }
        public string Duration { get; set; }
        public string? DistanceBtwnTruckAndPickup { get; set; }
        public string? DurationBtwnTruckAndPickup { get; set; }
        public ServiceStatus ServiceStatus { get; set; }
    }
}
