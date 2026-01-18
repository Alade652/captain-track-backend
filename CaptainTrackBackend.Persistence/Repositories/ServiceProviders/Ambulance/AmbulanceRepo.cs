using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.Ambulance;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.Ambulance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Persistence.Repositories.ServiceProviders.Ambulance
{
    public class AmbulanceRepo : RepositoryAsync<AmbulanceCompany>, IAmbulanceRepo
    {
        public AmbulanceRepo(ApplicationDbContext context) : base(context)
        {
        }
    }
}
