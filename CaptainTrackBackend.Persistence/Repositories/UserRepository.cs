using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace CaptainTrackBackend.Persistence.Repositories
{
    public class UserRepository : RepositoryAsync<User>, IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> Delete(Guid id)
        {
            await _context.Users.Where(x => x.Id == id).ExecuteDeleteAsync();
            return true;
        }

        public async Task<User> GetAsync(Guid id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == id);
            return user;
        }


        public async Task<User> GetUserAsync(Expression<Func<User, bool>> expression)
        {
            var ans = await _context.Users.Include(x => x.Customer).Include(x => x.UserServiceProvidings).ThenInclude(x => x.ServiceProviding).FirstOrDefaultAsync(expression);
            return ans;
        }
    }
}
