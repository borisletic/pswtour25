using Microsoft.AspNetCore.Mvc;
using TourApp.Application.Services;

namespace TourApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public TestController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("send-monthly-reports")]
        public async Task<IActionResult> TestMonthlyReports()
        {
            try
            {
                await _notificationService.SendMonthlyReportsAsync();
                return Ok("Monthly reports sent successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
