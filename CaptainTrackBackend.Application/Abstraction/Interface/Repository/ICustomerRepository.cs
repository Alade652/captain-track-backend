using System;

using CaptainTrackBackend.Domain.Entities;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Repository
{
    public interface ICustomerRepository : IRepositoryAsync<Customer>
    {
        Task<Customer> GetAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
    }
}
