using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services
{
    public interface ISmsService
    {
        Task<string> SendSmsAsync(string to, string message);
    }
}
