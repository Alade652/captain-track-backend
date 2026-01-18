using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Domain.Entities;
using CaptainTrackBackend.Domain.Identity;
using Microsoft.Extensions.Caching.Memory;
using System.Net.WebSockets;
using static System.Net.WebRequestMethods;
namespace CaptainTrackBackend.Application.Authentcication
{
    public class OTPService : IOTPService
    {
        public readonly IMemoryCache _cache;
        private readonly TimeSpan _otpExpiry = TimeSpan.FromMinutes(5);
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly ISmsService _smsService;
        public OTPService(IEmailService emailService, IUserRepository userRepository, IMemoryCache cache, ISmsService smsService)
        {
            _emailService = emailService;
            _userRepository = userRepository;
            _cache = cache;
            _smsService = smsService;
        }

        public async Task<Response<string>> GenerateOTP(Guid? userId = null, string? email = null /*string? phoneNumber = null*/)
        {
            var response = new Response<string>();
            string otp = new Random().Next(1000, 9999).ToString();
            if (userId != null)
            {
                var user = await _userRepository.GetAsync(x => x.Id == userId);
                _cache.Set(userId, otp, _otpExpiry); // cache with expiry

                string m = $"OTP : {otp}.";
                string message = m.ToString();

                //await _smsService.SendSmsAsync(user.PhoneNumber, m);

                var emailDto = new EmailDto
                {
                    To = user.Email,
                    Subject = "OTP",
                    Body = $"Hello ,<br/>. This is your OTP {otp}."
                };

                await _emailService.SendEmailAsync(emailDto);
            }
            else if (email != null)
            {
                var user = await _userRepository.GetAsync(x => x.Email == email);
                if (user == null) { response.Success = false; response.Message = "Email not registered"; return response; }
                _cache.Set(user.Id, otp, _otpExpiry);
                var emailDto = new EmailDto
                {
                    To = email,
                    Subject = "OTP",
                    Body = $"Hello ,<br/>. This is your OTP {otp}."
                };

                await _emailService.SendEmailAsync(emailDto);
            }
/*            else if (phoneNumber != null)
            {
                var user = await _userRepository.GetAsync(x => x.PhoneNumber == phoneNumber);
                if (user == null) { response.Success = false; response.Message = "PhoneNumber not registered"; return response; }
                _cache.Set(user.Id, otp, _otpExpiry);

                string m = $"OTP : {otp}.";
                string message = m.ToString();
                await _smsService.SendSmsAsync(phoneNumber, message);
            }*/

            response.Message = "OTP generates successfully";
            response.Success = true;
            response.Data = otp;
            return response;
        }
        public async Task<Response<string>> VarifyOTPAsync(string otp, Guid? userId = null, string? email = null/*string? phoneNumber = null*/)
        {
            var response = new Response<string>();

            if (userId != null)
            {
                var user = await _userRepository.GetAsync(x => x.Id == userId);
                if (_cache.TryGetValue(userId, out string storedOtp) && storedOtp == otp)
                {
                    _cache.Remove(userId); 
                    var emailDto = new EmailDto
                    {
                        To = user.Email,
                        Subject = "Welcome to CaptainTrack",
                        Body = $"Hello ,<br/>Thank you for registering with CaptainTrack. Your account has been created successfully."
                    };
                    await _emailService.SendEmailAsync(emailDto);
                    user.OTPVerfication = true;
                    await _userRepository.UpdateAsync(user);


                    response.Message = "OTP Verfied successfully";
                    response.Success = true;
                    response.Data = null;
                    return response;
                }
            }
            else if(email != null)
            {
                var user = await _userRepository.GetAsync(x => x.Email == email);
                if(user == null)
                {
                    response.Success = false;
                    response.Message = "Email not registered";
                    return response;
                }
                if (_cache.TryGetValue(user.Id, out string storedOtp) && storedOtp == otp)
                {
                    _cache.Remove(user.Id); // remove on successful verification
/*                    var emailDto = new EmailDto
                    {
                        To = user.Email,
                        Subject = "Welcome to CaptainTrack",
                        Body = $"Hello ,<br/>Thank you for registering with CaptainTrack. Your account has been created successfully."
                    };
                    await _emailService.SendEmailAsync(emailDto);
                    user.OTPVerfication = true;
                    await _userRepository.UpdateAsync(user);*/


                    response.Message = "OTP Verfied successfully";
                    response.Success = true;
                    response.Data = null;
                    return response;
                }
            }
            /*else if (phoneNumber != null)
            {
                var user = await _userRepository.GetAsync(x => x.PhoneNumber == phoneNumber);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "PhoneNumber";
                    return response;
                }
                if (_cache.TryGetValue(user.Id, out string storedOtp) && storedOtp == otp)
                {
                    _cache.Remove(userId); // remove on successful verification
                   *//* var emailDto = new EmailDto
                    {
                        To = user.Email,
                        Subject = "Welcome to CaptainTrack",
                        Body = $"Hello ,<br/>Thank you for registering with CaptainTrack. Your account has been created successfully."
                    };
                    await _emailService.SendEmailAsync(emailDto);
                    user.OTPVerfication = true;
                    await _userRepository.UpdateAsync(user);*//*


                    response.Message = "OTP Verfied successfully";
                    response.Success = true;
                    response.Data = null;
                    return response;
                }
            }*/

            response.Message = "Inavlid OTP ";
            response.Success = false;
            response.Data = null;
            return response;
        }


    }
}

