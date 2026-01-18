using CaptainTrackBackend.Domain.Entities.ServiceProviders.HouseCleaning;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.DTO.ServiceProviders.HouseCleaning
{
    public class HouseCleanerPackageDto
    {
        public Guid HouseCleanerPackageId { get; set; }
        public Guid? HouseCleanerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public List<HouseCleanerItemDto> HouseCleanerItems { get; set; } = new List<HouseCleanerItemDto>();
    }

    public class HouseCleanerPackageRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile ImageUrl { get; set; }
        //public List<HouseCleanerItemRequest> HouseCleanerItems { get; set; } = new List<HouseCleanerItemRequest>();
    }

    public class HouseCleanerItemRequest
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
    public class HouseCleanerItemDto
    {
        public Guid HouseCleanerItemId { get; set; }
        public Guid HouseCleanerPackageId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
