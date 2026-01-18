using CaptainTrackBackend.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.CarWash
{
    public class CarWashingitem : AuditableEntity
    {
        public Guid CarWashingId { get; set; }
        public CarWashing CarWashing { get; set; }
        public Guid CarWashItemId { get; set; }
        public CarWashItem CarWashItem { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

    }
}
