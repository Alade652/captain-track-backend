using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.Ambulance;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.Ambulance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Persistence.Repositories.ServiceProviders.Ambulance
{
    public class AmbulanceBookingRepo : RepositoryAsync<AmbulanceBooking>, IAmbulanceBookingRepo
    {
        private readonly ApplicationDbContext _context;
        public AmbulanceBookingRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AmbulanceBooking>> GetBookings(Expression<Func<AmbulanceBooking, bool>> expression)
        {
            var bookings = await _context.AmbulanceBookings.Include(x => x.AmbulanceCompany).Where(expression).ToListAsync();
            return bookings;
        }
    }
}
