using CaptainTrackBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.DTO.ServiceProviders.HouseCleaning
{
    public class HouseCleaningDto
    {
        public Guid HouseCleaningId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? HouseCleanerId { get; set; }
        public Guid HouseCleanerPackageId { get; set; }
        public string AddressorLocation { get; set; }
        public DateTime ServiceDate { get; set; }
        public decimal EstimatePrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string ServiceStatus { get; set; }
        public string? DistancetoLocation { get; set; }
        public string? DuraiontoLocation { get; set; }
        public List<HouseCleaningItemDto> HouseCleanerItems { get; set; } = new List<HouseCleaningItemDto>();
        public int ProvidersNotified { get; set; }
    }

    public class HouseCleaningRequest
    {
        public Guid HouseCleanerPackageId { get; set; }
        public string AddressorLocation { get; set; }
        public DateTime ServiceDate { get; set; }
        public List<HouseCleaningItemRequest> HouseCleaningItems { get; set; } = new List<HouseCleaningItemRequest>();
    }

    public class HouseCleaningItemDto
    {
        public Guid HouseCleaningItemId { get; set; }
        public Guid HouseCleaningId { get; set; }
        public Guid HouseCleanerItemId { get; set; }
        public string Name { get; set; }
        public decimal ItemsPrice { get; set; }
        public int Quantity { get; set; }
    }
    public class HouseCleaningItemRequest
    {
        public Guid HouseCleanerItemId { get; set; }
        public int Quantity { get; set; }
    }

}
