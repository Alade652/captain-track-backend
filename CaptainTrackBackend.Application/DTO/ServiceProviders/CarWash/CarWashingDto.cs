using CaptainTrackBackend.Domain.Entities.ServiceProviders.CarWash;
using CaptainTrackBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.DTO.ServiceProviders.CarWash
{
    public class CarWashingDto
    {
        public Guid CarWashingId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? CarWasherId { get; set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public decimal TotalPrice { get; set; }
        public string ServiceStatus { get; set; }
        public string? Location { get; set; }
        public string? DistancetoLocation { get; set; }
        public string? DurationtoLocation { get; set; }
        public List<CarWashingItemDto> CarWashingItems { get; set; } = new List<CarWashingItemDto>();
        public decimal EstimatedPrice { get;  set; }
        public int ProvidersNotified { get; set; }
    }

    public class CarWashingRequest
    {
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public string? Location { get; set; }
        public List<CarWashingItemRequest> CarWashingItems { get; set; } = new List<CarWashingItemRequest>();
    }

    public class CarWashingItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class CarWashingItemRequest
    {
        public Guid CarWashItemId { get; set; }
    }
}
