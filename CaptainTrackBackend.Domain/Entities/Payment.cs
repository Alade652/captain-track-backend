using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities
{
    public class Payment: AuditableEntity
    {
        public string Tx_ref { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "NGN";
        public string Redirect_url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public string PaymentLink { get; set; }
        public string? TransactionId { get; set; }
        public string Status { get; set; } // e.g., "successful", "failed", etc.

    }
}
