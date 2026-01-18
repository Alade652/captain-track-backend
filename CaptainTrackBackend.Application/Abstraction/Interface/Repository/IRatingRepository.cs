using CaptainTrackBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Repository
{
    public interface IRatingRepository : IRepositoryAsync<Rating>
    {
        Task<IList<Rating>> GetAllAsync(Expression<Func<Rating, bool>> expression);
    }
}
