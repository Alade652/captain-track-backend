using CaptainTrackBackend.Domain.Entities.ServiceProviders.Courier;
using CaptainTrackBackend.Domain.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.DTO.ServiceProviders.Courier
{
    public class ParkDto
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
        public string ServiceProviderRole { get; set; }
        public string VerificationStatus { get; set; }
        public List<CourierVehicleDto> Vehicles { get; set; } = new List<CourierVehicleDto>();
    }

    public class CourierVehicleDto
    {
        public Guid VehicleId { get; set; }
        public Guid? ParkId { get; set; }
        public string VehicleType { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }

    public class CourierVehicleRequest
    {
        public string VehicleType { get; set; }
        public decimal Price { get; set; }
        public IFormFile? Image { get; set; }
    }
}
