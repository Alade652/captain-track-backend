using CaptainTrackBackend.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.Courier
{
    public class CourierVehicle : AuditableEntity
    {
        public Guid? ParkId { get; set; }
        public RiderorPark Park { get; set; }
        public string VehicleType { get; set; } 
        public decimal Price { get; set; } 
        public string? ImageUrl { get; set; }
    }
}
