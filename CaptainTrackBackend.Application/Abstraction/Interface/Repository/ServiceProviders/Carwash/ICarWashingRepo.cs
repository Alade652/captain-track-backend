using CaptainTrackBackend.Domain.Entities.ServiceProviders.CarWash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.Carwash
{
    public interface ICarWashingRepo : IRepositoryAsync<CarWashing>
    {
        Task<IList<CarWashing>> GetAllByExpressionAsync(Expression<Func<CarWashing, bool>> expression);
    }
}
