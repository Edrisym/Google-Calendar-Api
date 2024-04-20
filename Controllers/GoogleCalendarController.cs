using System.Formats.Asn1;
using Google.Apis.Calendar.v3.Data;
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
            var createdEvent = await _service.CreateEvent(model);
            if (createdEvent != null)
                return Ok("Event calendar was successfully Created!");
            else
                return BadRequest("Creting event calendar failed!");
        }

        [HttpGet]
        [Route("/GoogleCalendar/Revoke")]
        public async Task<IActionResult> Revoke()
        {
            var statusCode = _service.RevokeToken();
            if (statusCode)
                return Ok("Revoked successfully");
            else
                return BadRequest("Revoking FAILED!");
        }
    }
}
