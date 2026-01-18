using CaptainTrackBackend.Domain.Entities.ServiceProviders.WaterSupply;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.WaterSupply
{
    public interface IWaterSupplingRepo : IRepositoryAsync<WaterSuppling>
    {
        Task<IEnumerable<WaterSuppling>> GetBookings(Expression<Func<WaterSuppling, bool>> expression);
    }
}
