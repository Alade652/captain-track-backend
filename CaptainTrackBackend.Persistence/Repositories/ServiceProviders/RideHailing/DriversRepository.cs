using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.RideHailing;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.RideHailing;
using Microsoft.EntityFrameworkCore;

namespace CaptainTrackBackend.Persistence.Repositories.ServiceProviders.RideHailing
{
    public class DriversRepository: RepositoryAsync<Driver>, IDriversRepository
    {
        private readonly ApplicationDbContext _context;
        public DriversRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<Driver> GetAsync(Guid id)
        {
            var driver = _context.Drivers.Include(x => x.User).SingleOrDefaultAsync(x => x.Id == id);
            return driver;
        }
    }
}
