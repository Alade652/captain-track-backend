using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.WaterSupply;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.WaterSupply;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Persistence.Repositories.ServiceProviders.WaterSupply
{
    public class WaterSupplingRepo : RepositoryAsync<WaterSuppling>, IWaterSupplingRepo
    {
        private readonly ApplicationDbContext _context;
        public WaterSupplingRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WaterSuppling>> GetBookings(Expression<Func<WaterSuppling, bool>> expression)
        {
            return await _context.WaterSupplings.Include(x => x.WaterSupplier).Where(expression).ToListAsync();
        }
    }
}
