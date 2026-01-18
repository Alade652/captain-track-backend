using CaptainTrackBackend.Domain.Entities.ServiceProviders.GasDelivery;
using CaptainTrackBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.DTO.ServiceProviders.GasDelivery
{
    public class GasDeliveryDto
    {
        public Guid Id { get; set; }
        public Guid? GasSupplierId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid GasSupllierId { get; set; }
        public string CustomerName { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Status { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? EstimateTotalAmount { get; set; }
        public int CylinderCount { get; set; }
        public string? DeliveryNote { get; set; }
        public string ServiceMethod { get; set; }
        public string? DistancetoLocation { get; set; }
        public string? DurationtoLocation { get; set; }
        public List<CylinderDto> Cylinders { get; set; } = new List<CylinderDto>();
        public int ProvidersNotified { get; set; }
    }

    public class GasDeliveryRequestDto
    {
        public string DeliveryAddress { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string? DeliveryNote { get; set; }
        public ServiceMethod ServiceMethod { get; set; }
        public List<CylinderRequestDto> Cylinders { get; set; } = new List<CylinderRequestDto>();
    }

    public class CylinderDto
    {
        public Guid? CylinderId { get; set; }
        public string CylinderType { get; set; }
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }
    }
    public class CylinderRequestDto
    {
        public string CylinderType { get; set; }
        public int Quantity { get; set; }
    }
}
