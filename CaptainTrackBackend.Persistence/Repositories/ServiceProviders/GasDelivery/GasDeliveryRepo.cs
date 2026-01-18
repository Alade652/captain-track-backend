using CaptainTrackBackend.Application.Abstraction.Interface.Repository.ServiceProviders.GasDelivery;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.GasDelivery;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Persistence.Repositories.ServiceProviders.GasDelivery
{
    public class GasDeliveryRepo : RepositoryAsync<GasDelivering>, IGasDeliveryRepo
    {
        private readonly ApplicationDbContext _context;
        public GasDeliveryRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GasDelivering>> GetAllByExpressionAsync(Expression<Func<GasDelivering, bool>> expression)
        {
            var deliveries = await _context.GasDeliveries.Include(x => x.Cylinders)
                .Include(x => x.GasSupplier)
                .Include(x => x.Customer)
                .Where(expression)
                .ToListAsync();
            return deliveries;
        }
    }
}
