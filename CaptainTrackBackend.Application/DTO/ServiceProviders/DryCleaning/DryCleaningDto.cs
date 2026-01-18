using CaptainTrackBackend.Domain.Entities.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.DTO.ServiceProviders.DryCleaning
{
    public class DryCleaningDto
    {
        public Guid DrycleaningId { get; set; }
        public Guid? DryCleanerId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid PackageId { get; set; }
        public string CustomerName { get; set; }
        public string? Location { get; set; }
        public DateTime DeliveryDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? EstimateTotalAmount { get; set; }
        public List<DryCleaningItemDto> DryCleaningItems { get; set; } = new List<DryCleaningItemDto>();
        public string Status { get; set; }
        public string? DistanceToPickupLocation { get; set; }
        public string? DurationToPickupLocation { get; set; }
        public int ProvidersNotified { get; set; }
    }

    public class DryCleaningRequestDto
    {
        public Guid PackageId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string? Location { get; set; }
        public List<DryCleaningItemRequestDto> DryCleaningItems { get; set; } = new List<DryCleaningItemRequestDto>();
    }
}
