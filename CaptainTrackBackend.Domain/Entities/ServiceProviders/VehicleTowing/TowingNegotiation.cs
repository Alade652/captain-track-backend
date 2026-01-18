using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.VehicleTowing
{
    public class TowingNegotiation : AuditableEntity
    {
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public Guid TowTruckOperatorId { get; set; }
        public TowTruckOperator TowTruckOperator { get; set; }
        public Guid TowingId { get; set; }
        public Towing Towing { get; set; }
        public NegotiationStatus Status { get; set; }
        public decimal NegotiatingPrice { get; set; }
        public decimal Acceptedprice { get; set; }
    }
}
