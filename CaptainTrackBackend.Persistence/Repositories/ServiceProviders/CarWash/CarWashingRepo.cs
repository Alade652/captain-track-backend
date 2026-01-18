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
    public class CarWashingRepo : RepositoryAsync<CarWashing>, ICarWashingRepo
    {
        private readonly ApplicationDbContext _context;
        public CarWashingRepo(ApplicationDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task<IList<CarWashing>> GetAllByExpressionAsync(Expression<Func<CarWashing, bool>> expression)
        {
            var get = await _context.CarWashings.Include(x => x.CarWasher).Include(x => x.CarWashingItems).Where(expression).ToListAsync();
            return get;
        }
    }
}
