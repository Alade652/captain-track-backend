using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Enum;
using CaptainTrackBackend.Domain.Identity;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.GasDelivery
{
    public class GasSupplier : AuditableEntity
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
        public decimal PricePerKg { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public bool IsAvailable { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public ServiceProviderRole ServiceProviderRole { get; set; }
    }
}
