using CaptainTrackBackend.Domain.Entities.ServiceProviders.Ambulance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.Ambulance
{
    public interface IAmbulanceBookingRepo : IRepositoryAsync<AmbulanceBooking>
    {
        Task<IEnumerable<AmbulanceBooking>> GetBookings(Expression<Func<AmbulanceBooking, bool>> expression);
    }
}
