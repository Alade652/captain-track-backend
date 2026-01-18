using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services;
using CaptainTrackBackend.Application.Authentcication;
using CaptainTrackBackend.Application.AuthService;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Domain.Enum;
using CaptainTrackBackend.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CaptainTrackBackend.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IOTPService _otpService;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IAuthService _authService;
        private readonly ITokenBlacklistRepository _tokenBlacklistRepository;
        private readonly ISmsService _smsService;
        ApplicationDbContext _context;

        public UserService(IConfiguration configuration, IOTPService otpService, IUserRepository userRepository, IEmailService emailService, IAuthService authService,
            ITokenBlacklistRepository tokenBlacklistRepository, ApplicationDbContext context, ISmsService smsService)
        {
            _configuration = configuration;
            _otpService = otpService;
            _userRepository = userRepository;
            _emailService = emailService;
            _authService = authService;
            _tokenBlacklistRepository = tokenBlacklistRepository;
            _context = context;
            _smsService = smsService;
        }

        public async Task<Response<LogInDto>> LogIn(string email, string password)
        {
            var user = await _userRepository.GetUserAsync(x => x.Email == email);
            
            // Check if user exists first
            if (user == null)
            {
                return new Response<LogInDto>
                {
                    Message = "Invalid email or password",
                    Success = false,
                    Data = null
                };
            }
            
            // Check password
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return new Response<LogInDto>
                {
                    Message = "Invalid email or password",
                    Success = false,
                    Data = null
                };
            }
            
            // Check OTP verification
            if (user.OTPVerfication == false)
            {
                return new Response<LogInDto>
                {
                    Message = "Your email has not been verified. Please verify your email with the OTP sent to you.",
                    Success = false,
                    Data = null
                };
            }

            var token = _authService.GenerateJwtToken(user);
            if(user.Role == Role.ServiceProvider)
            {
                return new Response<LogInDto>
                {
                    Message = "Login successful",
                    Success = true,
                    Data = new LogInDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Role = user.Role.ToString(),
                        ServiceproviderRole = user.ServiceProviderRole.ToString(),
                        Token = token,
                        Services = user.UserServiceProvidings.Select(sp => new ServiceProvidingDto
                        {
                            Id = sp.ServiceProviding.Id,
                            Name = sp.ServiceProviding.Name,
                            Description = sp.ServiceProviding.Description
                        }).ToList()
                    }
                };
            }

            return new Response<LogInDto>
            {
                Message = "Login successful",
                Success = true,
                Data = new LogInDto
                {
                    Id = user.Id,
                    customerId = user.Customer?.Id ?? Guid.Empty,
                    Role = user.Role.ToString(),
                    Email = user.Email,
                    Token = token
                }
            };
        }

      /*  public async Task<Response<bool>> ForgotPassword(string? email, string? phone)
        {
            var response = new Response<bool>();
            var user = await _userRepository.GetAsync(u => u.Email == email || u.PhoneNumber == phone);
            if (user == null)
            {
                response.Message = "User Not found";
                response.Success = false;
                return response;
            };

            var otp = await _otpService.GenerateOTP(user.Id);
            var message = $"OTP : {otp.Data}";
            if (email != null)
            {
                await _smsService.SendSmsAsync(phone, message);

                var emailDto = new EmailDto
                {
                    To = user.Email,
                    Subject = "Reset Password",
                    Body = message
                };
                await _emailService.SendEmailAsync(emailDto);
            }
            else if (phone != null)
            {
                await _smsService.SendSmsAsync(phone, message);
            }

            response.Message = "reset OTP sent";
            response.Success = true;
            return response;
        }*/

        public async Task<Response<bool>> RessetPassword(string emailorPhone, string password)
        {
            var response = new Response<bool>();
            var user = await _userRepository.GetAsync(u => u.Email == emailorPhone || u.PhoneNumber == emailorPhone);
            if (user == null)
            {
                response.Message = "User Not found";
                response.Success = false;
                return response;
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(password);
            await _userRepository.UpdateAsync(user);

            response.Message = "Password Updated";
            response.Success = true;
            return response;
        }

        public async Task<Response<string>> LogOut(string token)
        {
            try
            {
                // Validate token (optional, depending on requirements)
                if (string.IsNullOrEmpty(token))
                {
                    return new Response<string>
                    {
                        Message = "Password is required",
                        Success = false,
                        Data = null
                    };
                }
                // Get token expiration date (assuming _authService can parse JWT)
                var expiryDate = _authService.GetTokenExpiryDate(token);
                if (expiryDate == null || expiryDate < DateTime.UtcNow)
                {
                    return new Response<string>
                    {
                        Message = "Invalid or expired token",
                        Success = false,
                        Data = null
                    };
                }

                // Add token to blacklist (e.g., store in database or cache)
                await _tokenBlacklistRepository.AddAsync(token, expiryDate);

                return new Response<string>
                {
                    Message = "Logout successful",
                    Success = true,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new Response<string>
                {
                    Message = $"Logout failed: {ex.Message}",
                    Success = false,
                    Data = null
                };
            }
        }

        public async Task<Response<EmailverificationDto>> ServiceProviderEmailVerification(EmailverificationRequest request)
        {
            var user = await _userRepository.GetAsync(x => x.Email == request.Email);
            if (user != null && user.Role == Role.ServiceProvider)
            {
                return new Response<EmailverificationDto>
                {
                    Message = "You're resgistered, do you want to provide other services ?",
                    Success = false
                };
            }

            var newUser = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                PhoneNumber = request.PhoneNumber,
                Role = Role.ServiceProvider
            };

            await _userRepository.AddAsync(newUser);
            await _otpService.GenerateOTP(newUser.Id);

            return new Response<EmailverificationDto>
            {
                Message = "Await Otp Verification",
                Success = true,
                Data = new EmailverificationDto
                {
                    UserId = newUser.Id,
                    FullName = newUser.FullName,
                    Email = newUser.Email,
                    PhoneNumber = newUser.PhoneNumber,
                    Role = newUser.Role.ToString()
                }
            };
        }

        public async Task<Response<string>> AddService(string roleName, string description)
        {
            var response = new Response<string>();
            var serviceProviding = await _context.ServiceProvidings.FirstOrDefaultAsync(x => x.Name.ToLower() == roleName.ToLower());
            if (serviceProviding == null)
            {
                response.Message = "Service providing not found.";
                response.Success = true;
                return response;
            }
            var newRole = new ServiceProviding
            {
                Name = roleName,
                Description = description
            };
            await _context.ServiceProvidings.AddAsync(newRole);
            await _context.SaveChangesAsync();

            response.Message = "service added successfully.";
            response.Success = true;
            response.Data = newRole.Name;
            return response;
        }

        public async Task<Response<bool>> DeleteUser(Guid userId)
        {
            var response = new Response<bool>();
            var user = await _userRepository.GetAsync(x => x.Id == userId);
            if (user == null) {
                response.Message = "User not found.";
                response.Success = false;
                return response;
            }
            if (user.Role == Role.Admin)
            {
                response.Message = "Cannot delete an admin user.";
                response.Success = false;
                return response;
            }
            await _userRepository.Delete(userId);
            response.Message = "User deleted successfully.";
            response.Success = true;
            response.Data = true;
            return response;
        }
    }
}
