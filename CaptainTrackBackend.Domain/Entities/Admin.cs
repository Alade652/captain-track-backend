using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Identity;

namespace CaptainTrackBackend.Domain.Entities
{
    public class Admin : AuditableEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }

}
