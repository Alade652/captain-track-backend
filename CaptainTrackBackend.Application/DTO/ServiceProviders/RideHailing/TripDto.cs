using CaptainTrackBackend.Domain.Entities.ServiceProviders;
using CaptainTrackBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.RideHailing;

namespace CaptainTrackBackend.Application.DTO.ServiceProviders.Ridehailing
{
    public class TripDto
    {
        public Guid Id { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Distance { get; set; }
        public string Duration { get; set; }
        public decimal EstimateFare { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } 
        public IList<string> Stops { get; set; } = new List<string>();
        public Guid CustomerId { get; set; }
        public Guid? DriverId { get; set; }
        public Guid? CancelledBy { get; set; }
        public string? DistanceToPickup { get; set; }
        public string? DurationToPickup { get; set; }
        public int ProvidersNotified { get; set; }
    }

    public class  TripRequestDto
    {
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
    }
}
