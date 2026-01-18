using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.Courier;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.Courier;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Persistence.Repositories.ServiceProviders.Courier
{
    public class CourierRepo : RepositoryAsync<Courier_Service>, ICourierRepo
    {
        private readonly ApplicationDbContext _context;
        public CourierRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Courier_Service>> Gets(Expression<Func<Courier_Service, bool>> expression)
        {
            var get = await _context.Courier_Services.Include(x => x.RiderorPark).Where(expression).ToListAsync();
            return get;
        }
    }
}
