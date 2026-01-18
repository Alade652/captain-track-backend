using CaptainTrackBackend.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Maps
{
    public interface ITrackingHub
    {
        //Task<string> SendLocation(Guid userId, double lat, double lng);
        Task<Response<string>> SendLocation(Guid userId, double lat, double lng);
        Task<Response<LocationDto>> GetLocation(Guid userId);
    }
}
