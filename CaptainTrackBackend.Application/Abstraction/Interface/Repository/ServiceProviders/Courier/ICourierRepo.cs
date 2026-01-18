using CaptainTrackBackend.Domain.Entities.ServiceProviders.Courier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.Courier
{
    public interface ICourierRepo : IRepositoryAsync<Courier_Service>
    {
        Task<List<Courier_Service>> Gets(Expression<Func<Courier_Service, bool>> expression);
    }
}
