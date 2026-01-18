using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptainTrackBackend.Application.DTO;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services
{
    public interface IEmailService
    {
        Task<Response<EmailDto>> SendEmailAsync(EmailDto emailDto);
    }
}
