using CaptainTrackBackend.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.HouseCleaning
{
    public class HouseCleaningItem : AuditableEntity
    {
        public Guid HouseCleaningId { get; set; }
        public Housecleaning HouseCleaning { get; set; }
        public Guid HouseCleanerItemid { get; set; }
        public HouseCleanerItem HouseCleanerItem { get; set; }
        public string Name { get; set; }
        public decimal ItemsPrice { get; set; }
        public int Quantity { get; set; }
    }
}
