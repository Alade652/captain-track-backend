using CaptainTrackBackend.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.RideHailing
{
    public class Trip : AuditableEntity
    {
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Distance { get; set; }
        public string Duration { get; set; }
        public string? DistanceToPickup { get; set; }
        public string? DurationToPickup { get; set; }
        public TripStatus Status { get; set; } = TripStatus.Pending;
        public IList<string> Stops { get; set; } = new List<string>();
        public decimal EstimatedFare { get; set; }
        public decimal Price { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? DriverId { get; set; }
        public Customer Customer { get; set; }
        public Driver Driver { get; set; }
        public Guid? CancelledBy { get; set; }
    }
}
