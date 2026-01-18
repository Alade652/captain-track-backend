using CaptainTrackBackend.Domain.Entities;
using CaptainTrackBackend.Domain.Entities.Common;
using CaptainTrackBackend.Domain.Entities.ServiceProviders;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Domain.Enum;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;

namespace CaptainTrackBackend.Domain.Identity
{
    public class User:AuditableEntity
    {
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool OTPVerfication { get; set; } = false;
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordTokenExpiry { get; set; }
        public Admin Admin { get; set; }
        public DryCleaner DryCleaner { get; set; }
        public Customer Customer { get; set; }
        public Role Role { get; set; }
        //public ServiceProviding Service { get; set; }
        public ServiceProviderRole ServiceProviderRole { get; set; }
        public List<UserServiceProviding> UserServiceProvidings { get; set; } = new List<UserServiceProviding>();
        public IList<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
