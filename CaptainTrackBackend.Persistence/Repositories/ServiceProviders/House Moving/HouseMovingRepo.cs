using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.House_Moving;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.House_Moving;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Persistence.Repositories.ServiceProviders.House_Moving
{
    public class HouseMovingRepo : RepositoryAsync<HouseMoving>, IHouseMovingRepo
    {
        private readonly ApplicationDbContext _context;
        public HouseMovingRepo(ApplicationDbContext context) : base(context) 
        {
            _context = context;
        }

        public async Task<IList<HouseMoving>> GetHouseMovingsAsync(Expression<Func<HouseMoving, bool>> expression)
        {
            return await _context.HouseMovings.Include(x => x.HouseMover).Where(expression).ToListAsync();
        }
    }
}
