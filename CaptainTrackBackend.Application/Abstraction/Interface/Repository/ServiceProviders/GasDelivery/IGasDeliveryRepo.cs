using CaptainTrackBackend.Domain.Entities.ServiceProviders.GasDelivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.GasDelivery
{
    public interface IGasDeliveryRepo : IRepositoryAsync<GasDelivering>
    {
        Task<IEnumerable<GasDelivering>> GetAllByExpressionAsync(Expression<Func<GasDelivering, bool>> expression);
    }
}
