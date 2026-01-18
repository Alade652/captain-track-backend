using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders.Ridehailing;
using Microsoft.AspNetCore.Http;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.RideHailing
{
    public interface IDriversServices
    {
        Task<Response<DriverDto>> InitReg(DriverInitRequest initRequest);
        Task<Response<DriverDto>> Register(Guid userId, DriverRequestDto driverDto, IFormFileCollection files);
        Task<Response<DriverDto>> Update(Guid id, DriverUpdateDto driverDto);
        Task<Response<DriverDto>> Get(Guid id);
        Task<Response<DriverDto>> Get(string email);
        Task<Response<DriverDto>> GetByUserId(Guid userId);
        Task<Response<IList<DriverDto>>> GetAll();
        Task<Response<IList<DriverDto>>> GetAvailable();
        
        Task<Response<bool>> Delete(Guid id);
        Task<Response<bool>> Approve(Guid id);
    }
}
