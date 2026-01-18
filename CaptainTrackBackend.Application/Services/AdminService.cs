using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Domain.Entities;
using CaptainTrackBackend.Domain.Enum;
using CaptainTrackBackend.Domain.Identity;

namespace CaptainTrackBackend.Application.Services
{
    public class AdminService : IAdminServices
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        public AdminService(IAdminRepository adminRepository, IUserRepository userRepository, IEmailService emailService)
        {
            _adminRepository = adminRepository;
            _userRepository = userRepository;
            _emailService = emailService;
        }
        public async Task<Response<AdminDto>> Create(AdminDto adminDto)
        {
            var check =await  _adminRepository.ExistsAsync(x => x.User.Email == adminDto.Email);
            if (check == true)
            {
                return new Response<AdminDto>
                {
                    Message = "Admin already exists",
                    Success = false,
                    Data = null
                };
            }
            var user = new User
            {
                Email = adminDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(adminDto.Password),
                Role = Role.Admin
            };
            var addUser = _userRepository.AddAsync(user);
            var admin = new Admin
            {
                UserId = user.Id,
                FirstName = adminDto.FirstName,
                LastName = adminDto.LastName,
                Email = adminDto.Email,
                PhoneNumber = adminDto.PhoneNumber,
            };
            await _adminRepository.AddAsync(admin);
            var newAdmin = new AdminDto
            {
                id = admin.Id,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                Email = admin.User.Email,
                PhoneNumber = admin.PhoneNumber,
            };
            var emailDto = new EmailDto
            {
                To = user.Email,
                Subject = "Welcome to CaptainTrack",
                Body = $"Hello {admin.FirstName},<br/>You've been added as an admin on CaptainTrack."
            };
            _emailService.SendEmailAsync(emailDto);
            return new Response<AdminDto>
            {
                Message = "Admin registered successfully",
                Success = true,
                Data = newAdmin
            };
        }

        public Task<Response<IList<AdminDto>>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<Response<AdminDto>> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Response<AdminDto>> Update(Guid id, AdminDto adminDto)
        {
            throw new NotImplementedException();
        }
    }
}
