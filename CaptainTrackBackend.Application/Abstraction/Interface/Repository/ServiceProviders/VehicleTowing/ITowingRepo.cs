using CaptainTrackBackend.Domain.Entities.ServiceProviders.VehicleTowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.VehicleTowing
{
    public interface ITowingRepo : IRepositoryAsync<Towing>
    {
        Task<IEnumerable<Towing>> GetBookings(Expression<Func<Towing, bool>> expression);
    }
}
