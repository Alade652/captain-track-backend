using CaptainTrackBackend.Domain.Entities.ServiceProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.DTO.ServiceProviders.DryCleaning
{
    public class DryCleaningItemDto
    {
        public Guid Id { get; set; }
        public Guid? ItemId { get; set; }
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }
    }

    public class DryCleaningItemRequestDto
    {
        public Guid? ItemId { get; set; }
        public int Quantity { get; set; }
    }
}
