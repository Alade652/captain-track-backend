using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.GasDelivery;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.GasDelivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Persistence.Repositories.ServiceProviders.GasDelivery
{
    public class GasSupplierRepo : RepositoryAsync<GasSupplier>, IGasSupplierRepo
    {
        public GasSupplierRepo(ApplicationDbContext context) : base(context)
        {
        }
    }
}
