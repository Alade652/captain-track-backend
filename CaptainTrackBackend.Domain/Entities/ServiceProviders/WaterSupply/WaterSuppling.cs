using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.WaterSupply
{
    public class WaterSuppling : AuditableEntity
    {
        public Guid? WaterSupplierId { get; set; }
        public WaterSupplier WaterSupplier { get; set; }
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public decimal TotalPrice { get; set; }
        public int QuantityInLitres { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string DeliveryAddress { get; set; }
        public string? DistancetoLocation { get; set; }
        public string? DuraiontoLocation { get; set; }
        public decimal EstimateTotalAmount { get; set; }
        public decimal NegotiatingAmount { get; set; }
        public ServiceStatus DeliveryStatus { get; set; } 
    }
}
