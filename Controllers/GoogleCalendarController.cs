using System.Formats.Asn1;
using Microsoft.AspNetCore.Mvc;
using Sample.GoogleCalendarApi.Common.Model;
using Sample.GoogleCalendarApi.Services;

namespace Sample.GoogleCalendarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleCalendarController : ControllerBase
    {
        private readonly IGoogleCalendarService _service;

        public GoogleCalendarController(IGoogleCalendarService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("/GoogleCalendar/CreateEvent")]
        public async Task<IActionResult> CreateEvent(EventModel model)
        {
            return Ok(await _service.CreateEvent(model));
        }

        [HttpGet]
        [Route("/GoogleCalendar/Revoke")]
        public async Task<IActionResult> Revoke()
        {
            var statusCode = _service.RevokeToken();
            if (statusCode)
                return Ok();
            else
                return BadRequest();
        }
    }
}
