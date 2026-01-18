using CaptainTrackBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.DTO.ServiceProviders.House_Moving
{
    public class HouseMovingDto
    {
        public Guid HouseMovingId { get; set; }
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
        public string Status { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? HouseMoverId { get; set; }
        public Guid PackageId { get; set; }
        public Guid TruckId { get; set; }
        public int ProvidersNotified { get; set; }
    }

    public class HouseMovingRequest
    {
        public Guid PackageId { get; set; }
        public Guid TruckId { get; set; }
        public string PickupLocation { get; set; }
        public string DropoffLocation { get; set; }
        public DateTime? MovingDate { get; set; }
        public DateTime? ArrivalTime { get; set; }
    }
}
