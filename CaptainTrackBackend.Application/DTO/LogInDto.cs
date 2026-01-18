using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.DTO
{
    public class LogInDto
    {
        public Guid Id { get; set; }
        public Guid customerId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public string ServiceproviderRole { get; set; }
        public List<ServiceProvidingDto> Services { get; set; } = new List<ServiceProvidingDto>();

    }


    public class ResetPasswordDto
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class ServiceProvidingDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
