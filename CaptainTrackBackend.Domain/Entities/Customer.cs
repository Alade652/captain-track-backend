using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Identity;

namespace CaptainTrackBackend.Domain.Entities
{
    public class Customer : AuditableEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressorLocation { get; set; }  
        public Guid UserId { get; set; }
        public User User { get; set; }
        public IList<Transaction> Transactions { get; set; } = new List<Transaction>();
        public IList<Payment> Payments { get; set; } = new List<Payment>();
        public IList<Rating> Ratings { get; set; } = new List<Rating>();



    }
}
