using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.DTO.ServiceProviders.GasDelivery
{
    public class GasSupplierUpdateRequest
    {
        public string? CompanyName { get; set; }
        public string? RC { get; set; }
        public string? AddressorLocation { get; set; }
        public string? City { get; set; }
        public string? BusinessContact { get; set; }
        public string? Owner { get; set; }
        public string? BusinessLogo { get; set; }
        public int Locations { get; set; }
        public string? YearsOfService { get; set; }
        public decimal PricePerKg { get; set; }
        public string? AccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
    }
}
