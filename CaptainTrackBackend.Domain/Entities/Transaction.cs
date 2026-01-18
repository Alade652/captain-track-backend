using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities
{
    public class Transaction : AuditableEntity
    {
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid PaymentId { get; set; }
        public Payment Payment { get; set; }
        public string Type { get; set; }
    }
}
