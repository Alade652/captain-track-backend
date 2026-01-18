using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.DryCleaning
{
    public class DryClean : AuditableEntity
    {
        public Customer Customer { get; set; }
        public Guid CustomerId { get; set; }
        public DryCleaner DryCleaner { get; set; }
        public Guid? DryCleanerId { get; set; }
        public Guid PackageId { get; set; }
        public string? CustomerLocation { get; set; }
        public LaundryPackage Package { get; set; }
        public List<DryCleaningItem> DryCleaningItems { get; set; } = new List<DryCleaningItem>();
        public DateTime DeliveryDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? EstimateTotalAMount { get; set; }
        public decimal NegotiatingAmount { get; set; } 
        public ServiceStatus Status { get; set; }
        public string? DistancetoLocation { get; set; }
        public string? DuraiontoLocation { get; set; }
        public List<DryCleaningNegotiation> Negotiations { get; set; } = new List<DryCleaningNegotiation>();
    }
}
