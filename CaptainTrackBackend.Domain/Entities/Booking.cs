using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptainTrackBackend.Domain.Entities.Common;

namespace CaptainTrackBackend.Domain.Entities
{
    public class Booking: AuditableEntity
    {
        public Guid CustomerId { get; set; } 
        //public Guid ServiceProviderId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        //public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        // Navigation properties
        public  Customer Customer { get; set; }
        //public Vendor Vendor { get; set; } 
    }   
}
