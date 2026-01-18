using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.DryCleaning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Persistence.Repositories.ServiceProviders.DryCleaning
{
    public class LaundryPackageRepository : RepositoryAsync<LaundryPackage>, ILaundryPackageRepository
    {
        public LaundryPackageRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
