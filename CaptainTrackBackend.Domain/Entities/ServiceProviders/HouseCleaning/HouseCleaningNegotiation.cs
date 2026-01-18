using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.HouseCleaning
{
    public class HouseCleaningNegotiation : AuditableEntity
    {
        public Guid HouseCleaningId { get; set; }
        public Housecleaning HouseCleaning { get; set; }
        public Guid HouseCleanerId { get; set; }
        public HouseCleaner HouseCleaner { get; set; }
        public NegotiationStatus Status { get; set; }
        public decimal NegotiatingPrice { get; set; }
        public decimal Acceptedprice { get; set; }
        public TimeSpan TimeRequired { get; set; } 
    }
}
