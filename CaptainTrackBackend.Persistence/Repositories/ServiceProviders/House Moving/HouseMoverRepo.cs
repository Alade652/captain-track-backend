using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
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
    public class HouseMoverRepo : RepositoryAsync<HouseMover>, IHouseMoverRepo
    {
        private readonly ApplicationDbContext _context;
        public HouseMoverRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<HouseMover> GetHouseMoverAsync(Expression<Func<HouseMover, bool>> expression)
        {
            //var houseMover = await _context.HouseMover.Include(h => h.Hou)
            throw new NotImplementedException();
        }
    }
}
