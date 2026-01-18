using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptainTrackBackend.Application.DTO;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services
{
    public interface IUserService
    {
        Task<Response<LogInDto>> LogIn(string email, string password);
        Task<Response<string>> LogOut(string token);
        //Task<Response<bool>> ForgotPassword(string? email, string? phone);
        Task<Response<bool>> RessetPassword(string emailorphone, string password);
        Task<Response<EmailverificationDto>> ServiceProviderEmailVerification(EmailverificationRequest request);
        Task<Response<string>> AddService(string roleName, string description);
        Task<Response<bool>> DeleteUser(Guid userId);
    }
}
