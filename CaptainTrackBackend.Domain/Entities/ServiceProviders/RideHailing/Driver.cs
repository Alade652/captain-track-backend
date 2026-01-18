using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Enum;
using CaptainTrackBackend.Domain.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.RideHailing
{
    public class Driver : AuditableEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public string? NIN { get; set; }
        public string? LicenceExpiryDate { get; set; }
        public string? ImageUrl { get; set; }
        public string? LicenseUrl { get; set; }
        public string? PlateNumber { get; set; }
        public string? VehicleImageUrl { get; set; }
        public string? VehicleColor { get; set; }
        public string? VehicleModel { get; set; }
        public string? YearOfManufacture { get; set; }
        public string? VehicleRegistrationUrl { get; set; }
        public string? RoadWorthyCertificateUrl { get; set; }
        public string? AccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public bool IsApproved { get; set; } = false;
        public bool IsAvailable { get; set; } = true;
        public string? CurrentLocation { get; set; }

    }
}
