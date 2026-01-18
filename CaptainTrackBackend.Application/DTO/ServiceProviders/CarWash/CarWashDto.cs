using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.DTO.ServiceProviders.CarWash
{
    public class CarWashDto
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
        public IEnumerable<CarWashItemDto> CarWashItems { get; set; } = new List<CarWashItemDto>();
    }

    public class CarWashItemDto
    {
        public Guid Id { get; set; }
        public Guid? CarWasherId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class  CarWashItemRequest
    {
        public string? Name { get; set; }
        public decimal Price { get; set; }
    }
}
