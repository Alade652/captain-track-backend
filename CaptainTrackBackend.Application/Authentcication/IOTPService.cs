using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptainTrackBackend.Application.DTO;

namespace CaptainTrackBackend.Application.Authentcication
{
    public interface IOTPService
    {
        Task<Response<string>> GenerateOTP(Guid? userId = null, string? email = null/*, string? phoneNumber = null*/);
        Task<Response<string>> VarifyOTPAsync(string otp, Guid? userId = null, string? email = null /*string? phoneNumber = null*/);
        

    }
}
