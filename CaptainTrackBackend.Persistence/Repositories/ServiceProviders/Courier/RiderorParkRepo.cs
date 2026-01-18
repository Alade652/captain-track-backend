using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.Courier;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.Courier;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Persistence.Repositories.ServiceProviders.Courier
{
    public class RiderorParkRepo : RepositoryAsync<RiderorPark>, IRiderorParkRepo
    {
        private readonly ApplicationDbContext _context;
        public RiderorParkRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IList<RiderorPark>> GetAllRiderorParksAsync()
        {
            var response = await _context.RiderorParks.Include(x => x.CourierVehicles)
                .ToListAsync();
            return response;
        }

        public Task<RiderorPark> GetRiderorParkAsync(Expression<Func<RiderorPark, bool>> expression)
        {
            var response = _context.RiderorParks.Include(x => x.CourierVehicles).Include(x => x.User)
                .FirstOrDefaultAsync(expression);
            return response;
        }
    }
}
