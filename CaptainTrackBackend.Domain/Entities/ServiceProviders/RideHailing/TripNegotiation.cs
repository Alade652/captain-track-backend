using CaptainTrackBackend.Domain.Entities;
using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.RideHailing;
using CaptainTrackBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Services.ServiceProviders.RideHailing
{
    public class TripNegotiation : AuditableEntity
    {
        public decimal NegotiatingPrice { get; set; }
        public decimal Acceptedprice { get; set; }
        public NegotiationStatus Status { get; set; }
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public Guid DriverId { get; set; }
        public Driver Driver { get; set; }
        public Guid TripId { get; set; }
        public Trip Trip { get; set; }

    }
}
