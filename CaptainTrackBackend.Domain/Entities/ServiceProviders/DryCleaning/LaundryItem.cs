using CaptainTrackBackend.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.DryCleaning
{
    public class LaundryItem : AuditableEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public List<DryCleanerLaundryItem> DryCleanerItems { get; set; } = new List<DryCleanerLaundryItem>();
        public List<DryCleaningItem> DryCleaningItems { get; set; } = new List<DryCleaningItem>();
    }
}
