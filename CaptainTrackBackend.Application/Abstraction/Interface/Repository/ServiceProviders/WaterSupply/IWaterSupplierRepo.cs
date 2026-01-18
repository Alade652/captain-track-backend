using CaptainTrackBackend.Domain.Entities.ServiceProviders.WaterSupply;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.WaterSupply
{
    public interface IWaterSupplierRepo : IRepositoryAsync<WaterSupplier>
    {
    }
}
