using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.GasDelivery
{
    public class GasDelivering : AuditableEntity
    {
        public Customer Customer { get; set; }
        public Guid CustomerId { get; set; }
        public GasSupplier GasSupplier { get; set; }
        public Guid? GasSupplierId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public ServiceStatus Status { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal? EstimateTotalAmount { get; set; }
        public decimal NegotiatingAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public int CylinderCount { get; set; }
        public string? DeliveryNote { get; set; }
        public string? DistancetoLocation { get; set; }
        public string? DuraiontoLocation { get; set; }
        public List<Cylinder> Cylinders { get; set; } = new List<Cylinder>();
        public ServiceMethod ServiceMethod { get; set; } 
    }
}
