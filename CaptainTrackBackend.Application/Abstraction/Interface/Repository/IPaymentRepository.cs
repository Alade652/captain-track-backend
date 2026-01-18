using CaptainTrackBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Repository
{
    public interface IPaymentRepository : IRepositoryAsync<Payment>
    {
    }
}
