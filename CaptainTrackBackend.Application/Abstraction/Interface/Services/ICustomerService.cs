using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptainTrackBackend.Application.DTO;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services
{
    public interface ICustomerService
    {
        Task<Response<CustomerDto>> Register(CustomerRequestDto customerDto);
        Task<Response<CustomerDto>> Update(Guid id, CustomerUpdateDto customerUpdateDto);
        Task<Response<bool>> Delete(Guid id);
        Task<Response<CustomerDto>> Get(Guid id);
        Task<Response<CustomerDto>> GetByUserId(Guid userId);
        Task<Response<CustomerDto>> Get(string email);
        Task<Response<IList<CustomerDto>>> GetAll();
    }
}
