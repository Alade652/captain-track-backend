using CaptainTrackBackend.Domain.Entities;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.WaterSupply;
using CaptainTrackBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.DTO.ServiceProviders.WaterSupply
{
    public class WaterSupplingDto
    {
        public Guid WaterSupplingId { get; set; }
        public Guid? WaterSupplierId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal TotalPrice { get; set; } 
        public int QuantityInLitres { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string DeliveryAddress { get; set; }
        public string? DistancetoLocation { get; set; }
        public string? DuraiontoLocation { get; set; }
        public decimal EstimateTotalAmount { get; set; }
        public decimal NegotiatingAmount { get; set; }
        public string DeliveryStatus { get; set; }
        public int ProvidersNotified { get; set; }

    }

    public class WaterSupplingRequest
    {
        public int QuantityInLitres { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string DeliveryAddress { get; set; }
    }
}
