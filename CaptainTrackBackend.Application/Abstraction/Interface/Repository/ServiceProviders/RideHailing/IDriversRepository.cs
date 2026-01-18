using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.RideHailing;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.RideHailing
{
    public interface IDriversRepository : IRepositoryAsync<Driver>
    {
        Task<Driver> GetAsync(Guid id);
    }
}
