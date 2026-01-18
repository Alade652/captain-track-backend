using System.Collections.Concurrent;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services;
using CaptainTrackBackend.Application.Authentcication;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Domain.Entities;
using CaptainTrackBackend.Domain.Enum;
using CaptainTrackBackend.Domain.Identity;


namespace CaptainTrackBackend.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IOTPService _otpService;  
        public CustomerService(ICustomerRepository customerRepository, IUserRepository userRepository,IEmailService emailService, IOTPService oTPService)
        {
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _otpService = oTPService;
        }

        public async Task<Response<CustomerDto>> Register(CustomerRequestDto customerDto)
        {
            var check = await _customerRepository.ExistsAsync(x => x.Email == customerDto.Email);
            if (check == true)
            {
                return new Response<CustomerDto>
                {
                    Message = "Customer already exists",
                    Success = false,
                    Data = null
                };
            }
            var user = new User
            {
                FullName = $"{customerDto.FirstName} {customerDto.LastName}",
                Email = customerDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(customerDto.Password),
                PhoneNumber = customerDto.PhoneNumber,
                Role = Role.Customer
            };
            var addUser = await _userRepository.AddAsync(user);
            var customer = new Customer
            {
                UserId = user.Id,
                FirstName = customerDto.FirstName,
                LastName = customerDto.LastName,
                Email = customerDto.Email,
                PhoneNumber = customerDto.PhoneNumber,
                AddressorLocation = customerDto.Address,
            };

            await _customerRepository.AddAsync(customer);
            var newCustomer = new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.User.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.AddressorLocation,
                UserId = user.Id,
            };
            await _otpService.GenerateOTP(user.Id);
            return new Response<CustomerDto>
            {
                Message = "Customer registered, awaiting otp verification",
                Success = true,
                Data = newCustomer
            };
        }
        public async Task<Response<CustomerDto>> Update(Guid id, CustomerUpdateDto customerUpdateDto)
        {
            var customer =await _customerRepository.GetAsync(id);
            if (customer == null)
            {
                return new Response<CustomerDto>
                {
                    Message = "Customer not found",
                    Success = false,
                    Data = null
                };
            }

            customer.FirstName = customerUpdateDto.FirstName ?? customer.FirstName;
            customer.LastName = customerUpdateDto.LastName ?? customer.LastName;
            customer.Email = customerUpdateDto.Email ?? customer.Email;
            customer.PhoneNumber = customerUpdateDto.PhoneNumber ?? customer.PhoneNumber;
            customer.AddressorLocation = customerUpdateDto.Address ?? customer.AddressorLocation;
            customer.User.Email = customerUpdateDto.Email ?? customer.User.Email;
            customer.User.Password = customerUpdateDto.Password != null ? BCrypt.Net.BCrypt.HashPassword(customerUpdateDto.Password) : customer.User.Password;

            await _userRepository.UpdateAsync(customer.User);

            var updateCustomer = await _customerRepository.UpdateAsync(customer);
            return new Response<CustomerDto>
            {
                Message = "Customer updated successfully",
                Success = true,
                Data = new CustomerDto
                {
                    Id = customer.Id,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.User.Email,
                    PhoneNumber = customer.PhoneNumber,
                    Address = customer.AddressorLocation,
                    UserId = customer.UserId
                }
            };
        }
        public async Task<Response<bool>> Delete(Guid id)
        {
            var customer = await _customerRepository.GetAsync(id);
            await _customerRepository.DeleteAsync(id);
            await _userRepository.Delete(customer.UserId);

            return new Response<bool>
            {
                Message = "Customer deleted",
                Success = true,
                Data = false
            };
        }
        public async Task<Response<CustomerDto>> Get(Guid id)
        {
            var getCustomer = await _customerRepository.GetAsync(x => x.Id == id);
            if (getCustomer == null)
            {
                return new Response<CustomerDto>
                {
                    Message = "Customer not found",
                    Success = false,
                    Data = null
                };
            }
            var customerDto = new CustomerDto
            {
                Id = getCustomer.Id,
                FirstName = getCustomer.FirstName,
                LastName = getCustomer.LastName,
                Email = getCustomer.Email,
                PhoneNumber = getCustomer.PhoneNumber,
                Address = getCustomer.AddressorLocation,
                UserId = getCustomer.UserId
            };
            return new Response<CustomerDto>
            {
                Message = "Customer found successfully",
                Success = true,
                Data = customerDto
            };
        }
        public Task<Response<IList<CustomerDto>>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<Response<CustomerDto>> GetByUserId(Guid userId)
        {
            var getCustomer = await _customerRepository.GetAsync(x => x.UserId == userId);
            if (getCustomer == null)
            {
                return new Response<CustomerDto>
                {
                    Message = "Customer not found",
                    Success = false,
                    Data = null
                };
            }
            var customerDto = new CustomerDto
            {
                Id = getCustomer.Id,
                FirstName = getCustomer.FirstName,
                LastName = getCustomer.LastName,
                Email = getCustomer.Email,
                PhoneNumber = getCustomer.PhoneNumber,
                Address = getCustomer.AddressorLocation,
                UserId = getCustomer.UserId
            };
            return new Response<CustomerDto>
            {
                Message = "Customer found successfully",
                Success = true,
                Data = customerDto
            };
        }

        public async Task<Response<CustomerDto>> Get(string email)
        {
            var getCustomer = await _customerRepository.GetAsync(x => x.User.Email == email);
            if (getCustomer == null)
            {
                return new Response<CustomerDto>
                {
                    Message = "Customer not found",
                    Success = false,
                    Data = null
                };
            }
            var customerDto = new CustomerDto
            {
                Id = getCustomer.Id,
                FirstName = getCustomer.FirstName,
                LastName = getCustomer.LastName,
                Email = email,
                PhoneNumber = getCustomer.PhoneNumber,
                Address = getCustomer.AddressorLocation,
                UserId = getCustomer.UserId
            };
            return new Response<CustomerDto>
            {
                Message = "Customer found successfully",
                Success = true,
                Data = customerDto
            };
        }

        
    }
}
