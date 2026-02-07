using CaptainTrackBackend.Domain.Entities.ServiceProviders.DryCleaning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.DryCleaning
{
    public interface IDryCleaningRepository : IRepositoryAsync<DryClean>
    {
        Task<IEnumerable<DryClean>> GetAllByExpressionAsync(Expression<Func<DryClean, bool>> expression);
        Task<DryClean?> GetByIdWithDetailsAsync(Guid id);
    }
}
