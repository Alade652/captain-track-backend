using CaptainTrackBackend.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.HouseCleaning
{
    public class HouseCleanerPackage : AuditableEntity
    {
        public Guid? HouseCleanerId { get; set; }
        public HouseCleaner HouseCleaner { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public List<HouseCleanerItem> HouseCleanerItems { get; set; } = new List<HouseCleanerItem>();

    }
}
