using System.Formats.Asn1;
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
        [Route("/GoogleCalendar/CreateEvent")]
        public async Task<IActionResult> CreateEvent()
        {
            return Ok(await _googleCalendarService.CreateEvent());
        }

        [HttpGet]
        [Route("/GoogleCalendar/Revoke")]
        public async Task<IActionResult> Revoke()
        {
            var statusCode = _googleCalendarService.RevokeToken();
            if (statusCode)
                return Ok();
            else
                return BadRequest();
        }

    }
}
