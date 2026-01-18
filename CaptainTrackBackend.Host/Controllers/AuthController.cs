using CaptainTrackBackend.Application.Abstraction.Interface.Services;
using CaptainTrackBackend.Application.Authentcication;
using CaptainTrackBackend.Application.DTO;
using Microsoft.AspNetCore.Mvc;

namespace CaptainTrackBackend.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IOTPService _otpService;
        public AuthController(IUserService userService, IOTPService oTPService)
        {
            _userService = userService;
            _otpService = oTPService;
        }

        [HttpPost("serviceProviderEmailVerification")]
        public async Task<IActionResult> ServiceProviderEmailVerification(EmailverificationRequest request)
        {
            var result = await _userService.ServiceProviderEmailVerification(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var result = await _userService.LogIn(email, password);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("resetPassword/{email}")]
        public async Task<IActionResult> ResetPassword(string email, string password)
        {
            var result = await _userService.RessetPassword(email, password);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("genarateOTP")]
        public async Task<IActionResult> GenerateOTp(Guid? userId = null, string? email = null)
        {
            var result = await _otpService.GenerateOTP(userId, email);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string token)
        {
            var result = await _userService.LogOut(token);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("verifyOTP")]
        public async Task<IActionResult> VerifyOTP(string otp, Guid? userId = null, string? email = null)
        {
            var result = await _otpService.VarifyOTPAsync(otp, userId, email);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("deleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid userId)
        {
            var result = await _userService.DeleteUser(userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
