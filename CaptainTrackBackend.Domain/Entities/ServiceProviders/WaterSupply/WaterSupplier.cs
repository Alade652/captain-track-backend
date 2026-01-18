using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Enum;
using CaptainTrackBackend.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.WaterSupply
{
    public class WaterSupplier : AuditableEntity
    {
        public User User { get; set; }
        public Guid UserId { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyDescription { get; set; }
        public string RCorNIN { get; set; }
        public string AddressorLocation { get; set; }
        public string City { get; set; }
        public string? BusinessContact { get; set; }
        public string? Owner { get; set; }
        public string BusinessLogoorImageUrl { get; set; }
        public int Locations { get; set; }
        public string? YearsOfService { get; set; }
        public bool VerificatioStatus { get; set; }
        public decimal PricePerLitre { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public bool IsAvailable { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public ServiceProviderRole ServiceProviderRole { get; set; }
    }
}
