using CaptainTrackBackend.Domain.Entities.ServiceProviders.HouseCleaning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.HouseCleaning
{
    public interface IHouseCleaningRepo : IRepositoryAsync<Housecleaning>
    {
        Task<List<Housecleaning>> GetAllByExpressionAsync(Expression<Func<Housecleaning, bool>> expression);
    }
}
