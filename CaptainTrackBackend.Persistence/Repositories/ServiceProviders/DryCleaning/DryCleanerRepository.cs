using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.RideHailing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Persistence.Repositories.ServiceProviders.DryCleaning
{
    public class DryCleanerRepository : RepositoryAsync<DryCleaner>, IDryCleanerRepository
    {
        private readonly ApplicationDbContext _context;
        public DryCleanerRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<DryCleaner> GetAsync(Guid id)
        {
            var dryCleaner = _context.DryCleaners.Include(x => x.User).Include(x =>x.DryCleanerItems).ThenInclude(x => x.LaundryItem).SingleOrDefaultAsync(x => x.Id == id);
            return dryCleaner;
        }
    }
}
