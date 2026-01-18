using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.HouseCleaning;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.HouseCleaning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Persistence.Repositories.ServiceProviders.HouseCleaning
{
    public class HouseCleanerRepo : RepositoryAsync<HouseCleaner>, IHouseCleanerRepo
    {
        public HouseCleanerRepo(ApplicationDbContext context) : base(context) 
        {
        }
    }
}
