using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.DryCleaning;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Persistence.Repositories.ServiceProviders.DryCleaning
{
    public class DryCleaningRepository : RepositoryAsync<DryClean>, IDryCleaningRepository
    {
        ApplicationDbContext _context;
        public DryCleaningRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DryClean>> GetAllByExpressionAsync(Expression<Func<DryClean, bool>> expression)
        {
            var dryCleanings = await _context.DryCleanings.Include(x => x.DryCleaningItems).ThenInclude(dryCleanings => dryCleanings.LaundryItem)
                .Include(x => x.DryCleaner)
                .Include(x => x.Customer)
                .Where(expression)
                .ToListAsync();
            return dryCleanings;
        }
    }
}
