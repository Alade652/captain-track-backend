using CaptainTrackBackend.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.VehicleTowing
{
    public class Truck : AuditableEntity
    {
        public Guid? TowTruckOperatorId { get; set; }
        public TowTruckOperator TowTruckOperator { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string ImageUrl { get; set; }
    }
}
