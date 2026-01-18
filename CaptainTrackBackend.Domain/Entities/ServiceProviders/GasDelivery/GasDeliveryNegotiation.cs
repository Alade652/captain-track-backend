using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.GasDelivery
{
    public class GasDeliveryNegotiation : AuditableEntity
    {
        public Guid GasDeliveringId { get; set; }
        public Guid GasSupplierId { get; set; }
        public NegotiationStatus Status { get; set; } 
        public GasDelivering GasDelivering { get; set; }
        public GasSupplier GasSupplier { get; set; }
        public TimeSpan TimeRequired { get; set; }
        public decimal? NegotiatedPrice { get; set; }
        public decimal AcceptedPrice { get; set; }
    }
}
