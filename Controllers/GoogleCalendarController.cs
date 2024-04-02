using Google.Apis.Calendar.v3.Data;
using Microsoft.AspNetCore.Mvc;
using Sample.GoogleCalendarApi.Services;

namespace Sample.GoogleCalendarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleCalendarController : ControllerBase
    {
        private readonly IGoogleCalendarService _googleCalendarService;

        public GoogleCalendarController(IGoogleCalendarService googleCalendarService)
        {
            _googleCalendarService = googleCalendarService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent()
        {
            return Ok(await _googleCalendarService.CreateEvent());
        }
    }
}
