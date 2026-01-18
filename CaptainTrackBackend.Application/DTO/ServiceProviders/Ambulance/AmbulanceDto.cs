using CaptainTrackBackend.Domain.Enum;
using CaptainTrackBackend.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.DTO.ServiceProviders.Ambulance
{
    public class AmbulanceDto
    {
        public User User { get; set; }
        public string? CompanyName { get; set; }
        public string RCorNIN { get; set; }
        public string AddressorLocation { get; set; }
        public string City { get; set; }
        public string BusinessContact { get; set; }
        public string? Owner { get; set; }
        public string BusinessLogoorImageUrl { get; set; }
        public int Locations { get; set; }
        public string? YearsOfService { get; set; }
        public bool VerificatioStatus { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public decimal Price { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public ServiceProviding ServiceProviding { get; set; }
        public ServiceProviderRole ServiceProviderRole { get; set; }
    }
}
