using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Domain.Identity;

namespace CaptainTrackBackend.Application.AuthService
{
    public interface IAuthService
    {
        public string GenerateJwtToken(User user);
        public string GenerateResetOTP();
        public DateTime? GetTokenExpiryDate(string token);
    }
}
