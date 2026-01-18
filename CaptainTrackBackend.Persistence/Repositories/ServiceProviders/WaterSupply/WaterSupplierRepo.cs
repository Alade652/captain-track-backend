using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.WaterSupply;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.WaterSupply;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Persistence.Repositories.ServiceProviders.WaterSupply
{
    public class WaterSupplierRepo : RepositoryAsync<WaterSupplier>, IWaterSupplierRepo
    {
        public WaterSupplierRepo(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
