using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities;

namespace CaptainTrackBackend.Persistence.Repositories
{
    public class AdminRepository: RepositoryAsync<Admin>, IAdminRepository
    {
        public AdminRepository(ApplicationDbContext context) : base(context)
        {
        }

    }
}
