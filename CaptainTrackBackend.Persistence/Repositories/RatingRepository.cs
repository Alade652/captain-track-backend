using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Persistence.Repositories
{
    public class RatingRepository : RepositoryAsync<Rating>, IRatingRepository
    {
        private readonly ApplicationDbContext _context;
        public RatingRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Rating> GetAsync(Guid id)
        {
            var rating = await _context.Ratings.Include(x=> x.Customer).Include(x => x.User).SingleOrDefaultAsync(x => x.Id == id);
            return rating;
        }

        public async Task<IList<Rating>> GetAllAsync(Expression<Func<Rating, bool>> expression)
        {
            var ratings = await _context.Ratings.Include(x => x.Customer).Include(x => x.User).Where(expression).ToListAsync();
            return ratings;
        }
/*        public async Task<IList<Rating>> GetAllAsync(Guid customerId, Guid vendorId)
        {
            var ratings = await _context.Ratings.Include(x => x.Customer).Include(x => x.User).Where(x => x.CustomerId == customerId && x.UserId == vendorId).ToListAsync();
            return ratings;
        }*/
    }
}
