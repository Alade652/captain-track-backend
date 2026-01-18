using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.House_Moving
{
    public class HouseMovingNegotiation : AuditableEntity
    {
        public Guid HouseMovingId { get; set; }
        public Guid HouseMoverId { get; set; }
        public HouseMoving HouseMoving { get; set; }
        public HouseMover HouseMover { get; set; }
        public NegotiationStatus Status { get; set; }
        public decimal NegotiatingPrice { get; set; }
        public decimal Acceptedprice { get; set; }
    }
}
