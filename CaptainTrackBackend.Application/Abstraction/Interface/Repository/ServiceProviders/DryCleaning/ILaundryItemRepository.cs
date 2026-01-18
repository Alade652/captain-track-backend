using CaptainTrackBackend.Domain.Entities.ServiceProviders;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.DryCleaning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.DryCleaning
{
    public interface ILaundryItemRepository : IRepositoryAsync<LaundryItem>
    {
    }
}
