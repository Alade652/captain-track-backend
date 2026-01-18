using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptainTrackBackend.Domain.Entities;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Repository
{
    public interface IAdminRepository: IRepositoryAsync<Admin>
    {
    }
}
