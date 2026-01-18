using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Enum;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.HouseCleaning
{
    public class Housecleaning : AuditableEntity
    {
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public Guid? HouseCleanerId { get; set; }
        public HouseCleaner HouseCleaner { get; set; }
        public Guid HouseCleanerPackageId { get; set; }
        public HouseCleanerPackage HouseCleanerPackage { get; set; }
        public List<HouseCleaningItem> HouseCleaningItems { get; set; } = new List<HouseCleaningItem>();
        public string AddressorLocation { get; set; }
        public decimal EstimatedPrice { get; set; }
        public decimal NegotiatedPrice{ get; set; }
        public decimal TotalPrice { get; set; }
        public string? Location { get; set; }
        public string? DistancetoLocation { get; set; }
        public string? DuraiontoLocation { get; set; }
        public DateTime ServiceDate { get; set; }
        public ServiceStatus ServiceStatus { get; set; }

    }
}

