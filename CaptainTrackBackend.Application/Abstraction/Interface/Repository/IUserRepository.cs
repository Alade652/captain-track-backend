using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CaptainTrackBackend.Domain.Identity;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Repository
{
    public interface IUserRepository : IRepositoryAsync<User>
    {
        Task<bool> Delete(Guid id);
        Task<User> GetAsync(Guid id);
        Task<User> GetUserAsync(Expression<Func<User, bool>> expression);

    }
}
