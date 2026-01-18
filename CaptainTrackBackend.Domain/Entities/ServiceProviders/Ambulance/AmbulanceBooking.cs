using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Enum;
using MimeKit.Encodings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.Ambulance
{
    public class AmbulanceBooking : AuditableEntity
    {
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public Guid? AmbulanceCompanyId { get; set; }
        public AmbulanceCompany AmbulanceCompany { get; set; }
        public string PickupLocation { get; set; }
        public string Destination { get; set; }
        public ServiceStatus ServiceStatus { get; set; }
        public string Distance { get; set; }
        public string Duration { get; set; }
        public string? DistanceToPickupLocation { get; set; }
        public string? DurationToPickupLocation { get; set; }
        public decimal EstimatedPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal NegotiatedPrice { get; set; }
    }
}
