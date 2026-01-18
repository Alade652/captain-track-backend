using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.Carwash;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.CarWash;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Persistence.Repositories.ServiceProviders.CarWash
{
    public class CarWasherRepo : RepositoryAsync<CarWasher>, ICarWasherRepo
    {
        private readonly ApplicationDbContext _context;
        public CarWasherRepo(ApplicationDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task<CarWasher> Get(Expression<Func<CarWasher, bool>> expression)
        {
            var carWasher = await _context.CarWashers.Include(x => x.CarWashItems).FirstOrDefaultAsync(expression);
            return carWasher;
        }

        public async Task<IEnumerable<CarWasher>> GetAll(/*Expression<Func<CarWasher, bool>> expression*/)
        {
            var carWashers = await _context.CarWashers.Include(x => x.CarWashItems).ToListAsync();
            return carWashers;
        }
    }
}
