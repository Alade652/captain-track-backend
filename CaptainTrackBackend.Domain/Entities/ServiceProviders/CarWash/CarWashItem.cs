using CaptainTrackBackend.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.CarWash
{
    public class CarWashItem : AuditableEntity
    {
        public Guid? CarWasherId { get; set; }
        public CarWasher CarWasher { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public IEnumerable<CarWashingitem> CarWashingItems { get; set; } = new List<CarWashingitem>();
    }
}
