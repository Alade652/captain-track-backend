using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.DTO.ServiceProviders.VehicleTowing
{
    public class TowingDto
    {
        public Guid TowingId { get; set; }
        public Guid? TowTruckOperatorId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid TruckId { get; set; }
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
        public string ServiceStatus { get; set; } 
        public int ProvidersNotified { get; set; }
    }

    public class TowingRequest
    {
        public Guid TruckId { get; set; }
        public string PickupLocation { get; set; }
        public string DropOffLocation { get; set; }
        public string CarModel { get; set; }
    }
}
