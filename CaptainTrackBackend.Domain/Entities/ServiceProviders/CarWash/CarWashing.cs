using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.CarWash
{
    public class CarWashing : AuditableEntity
    {
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public Guid? CarWasherId { get; set; }
        public CarWasher CarWasher { get; set; }
        public List<CarWashingitem> CarWashingItems { get; set; } = new List<CarWashingitem>();
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public decimal EstimatedPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal NegotiatedPrice { get; set; }
        public string? Location { get; set; }
        public string? DistancetoLocation { get; set; }
        public string? DuraiontoLocation { get; set; }
        public ServiceStatus ServiceStatus { get; set; }

    }
}
