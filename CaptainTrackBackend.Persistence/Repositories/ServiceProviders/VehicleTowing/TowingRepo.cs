using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.VehicleTowing;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.VehicleTowing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Persistence.Repositories.ServiceProviders.VehicleTowing
{
    public class TowingRepo : RepositoryAsync<Towing>, ITowingRepo
    {
        private readonly ApplicationDbContext _context;
        public TowingRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Towing>> GetBookings(Expression<Func<Towing, bool>> expression)
        {
            return await _context.Towings.Include(x => x.TowTruckOperator).Where(expression).ToListAsync();
        }
    }
}
