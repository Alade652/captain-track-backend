using CaptainTrackBackend.Domain.Entities.Common;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.DryCleaning
{
    public class DryCleaningItem : AuditableEntity
    {
        public Guid? DryCleanerLaundryItemId { get; set; }
        public DryCleanerLaundryItem DryCleanerLaundryItem { get; set; }
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }
        public Guid DryCleanId { get; set; }
        public DryClean DryClean { get; set; }
        public Guid? LaundryItemId { get; set; }
        public LaundryItem LaundryItem { get; set; }
    }
}
