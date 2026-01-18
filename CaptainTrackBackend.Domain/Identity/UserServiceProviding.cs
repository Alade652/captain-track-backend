using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Identity
{
    public class UserServiceProviding : AuditableEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid ServiceProvidingId { get; set; }
        public ServiceProviding ServiceProviding { get; set; }
    }
}
