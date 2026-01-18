using CaptainTrackBackend.Domain.Entities.ServiceProviders.RideHailing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.RideHailing
{
    public interface ITripRepository : IRepositoryAsync<Trip>
    {
        Task<Trip> GetAsync(Guid tripId);
        Task<IList<Trip>> GetTrips(Expression<Func<Trip, bool>> expression);
    }
}
