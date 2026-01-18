using CaptainTrackBackend.Application.Abstraction.Interface.Services;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.MapModels;
using CaptainTrackBackend.Application.Services.FileUpload;
using CaptainTrackBackend.Application.Services.ServiceProviders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CaptainTrackBackend.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IFileUploadService _cloudinaryService;
        private readonly ISmsService _smsService;
        private readonly IHubContext<NegotiationHub> _hubContext;

        public TestController(IEmailService emailService, IFileUploadService cloudinaryService, ISmsService smsService, IHubContext<NegotiationHub> hubContext)
        {
            _emailService = emailService;
            _cloudinaryService = cloudinaryService;
            _smsService = smsService;
            _hubContext = hubContext;
        }


        /*       [HttpPost("upload")]
               public async Task<IActionResult> UploadFile(IFormFile file)
               {
                   if (file == null || file.Length == 0)
                       return BadRequest("No file uploaded.");

                   var url = await _cloudinaryService.UploadAsync(file);
                   return Ok(new { url });
               }*/
        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail([FromBody] EmailDto emailDto)
        {
            var result = await _emailService.SendEmailAsync(emailDto);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpPost("send-sms")]
        public async Task<IActionResult> SendSms(string toPhoneNumber, string message)
        {
            try
            {
                var response = await _smsService.SendSmsAsync(toPhoneNumber, message);
                return Ok(new { success = true, response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendNegotiation(Guid userId, Guid bookingId, decimal offer)
        {
            // Send negotiation to passenger via SignalR
            await _hubContext.Clients.User(userId.ToString())
                .SendAsync("ReceiveNegotiation", bookingId, offer);

            return Ok(new { status = "Negotiation sent" });
        }


    }
}
