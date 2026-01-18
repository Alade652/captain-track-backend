using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.HouseCleaning;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.HouseCleaning;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Persistence.Repositories.ServiceProviders.HouseCleaning
{
    public class HouseCleaningRepo : RepositoryAsync<Housecleaning>, IHouseCleaningRepo
    {
        private readonly ApplicationDbContext _dbContext;
        public HouseCleaningRepo(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Housecleaning>> GetAllByExpressionAsync(Expression<Func<Housecleaning, bool>> expression)
        {
            var houseCleanings = await _dbContext.HouseCleanings.Include(x => x.HouseCleaner).Include(x => x.HouseCleaningItems)
                .Where(expression)
                .ToListAsync();
            return houseCleanings;
        }
    }
}
