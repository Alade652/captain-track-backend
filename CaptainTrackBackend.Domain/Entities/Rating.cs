using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Enum;
using CaptainTrackBackend.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities
{
    public class Rating: AuditableEntity
    {
        public int Stars { get; set; }
        public string Comment { get; set; }
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string ServiceType { get; set; }
        public Guid CreatedFor { get; set; }

    }
}
