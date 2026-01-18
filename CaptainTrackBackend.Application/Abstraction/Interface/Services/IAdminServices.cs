using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptainTrackBackend.Application.DTO;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services
{
    public interface IAdminServices
    {
        Task<Response<AdminDto>> Create(AdminDto adminDto);
        Task<Response<AdminDto>> Update(Guid id, AdminDto adminDto);
        Task<Response<AdminDto>> GetById(Guid id);
        Task<Response<IList<AdminDto>>> GetAll();
    }
}
