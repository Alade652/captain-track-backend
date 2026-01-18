using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.Ambulance
{
    public class AmbulanceNegotiation : AuditableEntity
    {
        public Guid BookingId { get; set; }
        public AmbulanceBooking Booking { get; set; }
        public Guid AmbulanceCompanyId { get; set; }
        public AmbulanceCompany AmbulanceCompany { get; set; }
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public NegotiationStatus Status { get; set; }
        public decimal NegotiatingPrice { get; set; }
        public decimal Acceptedprice { get; set; }
    }
}
