using CaptainTrackBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.DTO.ServiceProviders.Ambulance
{
    public class AmbulanceBookingDto
    {
        public Guid AmbulanceBookingId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? AmbulanceId { get; set; }
        public string PickupLocation { get; set; }
        public string Destination { get; set; }
        public string Distance { get;  set; }
        public string Duration { get; set; }
        public string? DistanceToPickupLocation { get;  set; }
        public string? DurationToPickupLocation { get;  set; }
        public decimal EstimatedPrice { get; internal set; }
        public decimal TotalPrice { get; internal set; }
        public string ServiceStatus { get; internal set; }

        public int ProvidersNotified { get; set; }
    }

    public class AmbulanceBookingRequest
    {
        public string PickupLocation { get; set; }
        public string Destination { get; set; }
    }
}
