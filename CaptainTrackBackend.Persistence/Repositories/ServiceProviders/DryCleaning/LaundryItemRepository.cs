using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities.ServiceProviders;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.DryCleaning;

namespace CaptainTrackBackend.Persistence.Repositories.ServiceProviders.DryCleaning
{
    public class LaundryItemRepository : RepositoryAsync<LaundryItem>, ILaundryItemRepository
    {
        private readonly ApplicationDbContext _context;
        public LaundryItemRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
