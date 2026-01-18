using CaptainTrackBackend.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.GasDelivery
{
    public class Cylinder : AuditableEntity
    {
        public Guid GasDeliveryId { get; set; }
        public GasDelivering GasDelivery { get; set; }
        public string CylinderType { get; set; }
        public int AmountOfGas { get; set; }
        public decimal Price { get; set; }
    }
}
