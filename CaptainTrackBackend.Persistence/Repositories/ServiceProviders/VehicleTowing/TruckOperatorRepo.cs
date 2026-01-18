using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.VehicleTowing;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.VehicleTowing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Persistence.Repositories.ServiceProviders.VehicleTowing
{
    public class TruckOperatorRepo : RepositoryAsync<TowTruckOperator>, ITruckOperatorRepo
    {
        private readonly ApplicationDbContext _context;
        public TruckOperatorRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<TowTruckOperator> Get(Expression<Func<TowTruckOperator, bool>> expression)
        {
            var truckOperator = await _context.TowTruckOperators
                .Include(t => t.trucks)
                .SingleOrDefaultAsync(expression);
            return truckOperator;
        }

        public async Task<IList<TowTruckOperator>> GetAll()
        {
            var truckOperators = await _context.TowTruckOperators
                .Include(t => t.trucks)
                .ToListAsync();
            return truckOperators;

        }
    }
}
