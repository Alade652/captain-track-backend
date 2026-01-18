using CaptainTrackBackend.Domain.Entities.ServiceProviders.House_Moving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.House_Moving
{
    public interface IHouseMoverRepo : IRepositoryAsync<HouseMover>
    {
        Task<HouseMover> GetHouseMoverAsync(Expression<Func<HouseMover, bool>> expression);
    }
}
