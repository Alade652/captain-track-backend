using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CaptainTrackBackend.Persistence.Repositories
{
    public class CustomerRepository : RepositoryAsync<Customer>, ICustomerRepository
    {
        private readonly ApplicationDbContext _context;
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Customer> GetAsync(Guid id)
        {
            var customer = await  _context.Customers.Include(x => x.User).SingleOrDefaultAsync(x => x.Id == id);
            return customer;
        }

        public async Task<bool> DeleteAsync( Guid id)
        {
            await _context.Customers.Where(x => x.Id == id).ExecuteDeleteAsync();
            return true;
        }

    }
}
