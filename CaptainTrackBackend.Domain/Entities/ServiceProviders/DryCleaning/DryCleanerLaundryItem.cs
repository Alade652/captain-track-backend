using CaptainTrackBackend.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.DryCleaning
{
    //Relationship between Drycleaner and LaundryItem
    public class DryCleanerLaundryItem : AuditableEntity
    {
        public Guid LaundryItemId { get; set; }
        public LaundryItem LaundryItem { get; set; }
        public decimal Price { get; set; }
        public Guid DrycleanerId { get; set; }
        public DryCleaner DryCleaner { get; set; }
        public List<DryCleaningItem> DryCleaningItem { get; set; } = new List<DryCleaningItem>();
    }
}
