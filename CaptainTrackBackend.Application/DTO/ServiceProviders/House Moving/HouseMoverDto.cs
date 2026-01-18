using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.DTO.ServiceProviders.House_Moving
{
    public class HouseMoverDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string CompanyName { get; set; }
        public string RC { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string CompanyContact { get; set; }
        public string Owner { get; set; }
        public string BusinessLogoUrl { get; set; }
        public int Locations { get; set; }
        public string YearsOfService { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string ServiceProviding { get; set; }
        public string ServiceProviderRole { get; set; }
        public IEnumerable<HouseMoverPackageDto> Packages { get; set; }
        public IEnumerable<HouseMovingTruckDto> houseMovingTrucks { get; set; }

    }

    public class HouseMoverPackageDto
    {
        public Guid PackageId { get; set; }
        public Guid? HouseMoverId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

    }

    public class HouseMoverPackageRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile? Image { get; set; }
    }

    public class HouseMovingTruckDto 
    {
        public Guid TruckId{ get; set; }
        public Guid? HouseMoverId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class HouseMovingTruckRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public IFormFile? Image { get; set; }
    }




}
