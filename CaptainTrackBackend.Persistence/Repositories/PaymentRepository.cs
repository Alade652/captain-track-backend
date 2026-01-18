using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities;

namespace CaptainTrackBackend.Persistence.Repositories
{
    public class PaymentRepository : RepositoryAsync<Payment>, IPaymentRepository
    {
        private readonly ApplicationDbContext _context;
        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        // Add any additional methods specific to Payment repository here
    }
}
