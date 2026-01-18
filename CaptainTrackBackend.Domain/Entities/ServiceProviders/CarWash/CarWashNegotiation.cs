using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.CarWash
{
    public class CarWashNegotiation : AuditableEntity
    {
        public Guid CarWashingId { get; set; }
        public CarWashing CarWashing { get; set; }
        public Guid CarWasherId { get; set; }
        public CarWasher CarWasher { get; set; }
        public NegotiationStatus Status { get; set; }
        public decimal NegotiatedPrice { get; set; } 
        public decimal Acceptedprice { get; set; }
        public TimeSpan TimeRequired { get; set; }
    }
}
