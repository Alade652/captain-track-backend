using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.RideHailing;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.RideHailing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Persistence.Repositories.ServiceProviders.RideHailing
{
    public class TripRepository : RepositoryAsync<Trip>, ITripRepository
    {
        private readonly ApplicationDbContext _context;
        public TripRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Trip> GetAsync(Guid tripId)
        {
            var trip = await _context.Trips
                .Include(x => x.Customer)
                .Include(x => x.Driver)
                .SingleOrDefaultAsync(x => x.Id == tripId);
            return trip;
        }

        public async Task<IList<Trip>> GetTrips(Expression<Func<Trip, bool>> expression)
        {
            return await _context.Trips
               .Include(x => x.Customer)
               .Include(x => x.Driver).Where(expression).ToListAsync();
        }
    }
}
