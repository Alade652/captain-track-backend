using CaptainTrackBackend.Domain.Entities.ServiceProviders.Courier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.Courier
{
    public interface IRiderorParkRepo : IRepositoryAsync<RiderorPark>
    {
        Task<RiderorPark> GetRiderorParkAsync(Expression<Func<RiderorPark, bool>> expression);
        Task<IList<RiderorPark>> GetAllRiderorParksAsync();
        
    }
}
