using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.DTO.ServiceProviders
{
    public class ServiceProviderDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string CompanyName { get; set; }
        public string RC { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string CompanyContact { get; set; }
        public string Owner { get; set; }
        public string? BusinessLogoUrl { get; set; }
        public int Locations { get; set; }
        public string YearsOfService { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string ServiceProviding { get; set; }
        public string ServiceProviderRole { get; set; }
        public decimal Price { get; set; }

    }

    public class ServiceProviderRequest
    {
        public string CompanyName { get; set; }
        public string RC { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string? CompanyContact { get; set; }
        public string Owner { get; set; }
        public string? BusinessLogoUrl { get; set; }
        public int Locations { get; set; }
        public string YearsOfService { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
    }

    public class FreelancerDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string NIN { get; set; }
        public string Location { get; set; }
        public string City { get; set; }
        public string? BusinessContact { get; set; }
        public string ImageUrl { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string ServiceProviding { get; set; }
        public string ServiceProviderRole { get; set; }
    }

    public class FreelancerRequest
    {
        public string NIN { get; set; }
        public string Location { get; set; }
        public string City { get; set; }
        public string? BusinessContact { get; set; }
        public string ImageUrl { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
    }

}
