using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.Courier
{
    public class CourierNegotiation : AuditableEntity
    {
        public Guid Courier_ServiceId { get; set; }
        public Courier_Service Courier_Service { get; set; }
        public Guid RiderorParkId { get; set; }
        public RiderorPark RiderorPark { get; set; }
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public NegotiationStatus Status { get; set; }
        public decimal NegotiatingPrice { get; set; }
        public decimal Acceptedprice { get; set; }
    }
}
