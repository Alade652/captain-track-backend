using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Persistence.Repositories
{
    public class TransactionRepository : RepositoryAsync<Transaction>, ITransactionRepository
    {
        public TransactionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
