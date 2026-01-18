using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.DryCleaning
{
    public class DryCleaningNegotiation : AuditableEntity
    {
        public Guid DryCleanId { get; set; }
        public DryClean DryClean { get; set; }
        public Guid DryCleanerId { get; set; }
        public NegotiationStatus Status { get; set; } 
        public decimal? NegotiatedPrice { get; set; }
        public decimal Acceptedprice { get; set; }
        public TimeSpan TimeRequired { get; set; }
    }
}
