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

        [HttpPost("CreateEvent")]
        public IActionResult CreateEvent([FromBody] EventModel model)
        {

            var createdEvent = _service.CreateEvent(model);
            if (createdEvent != null)
                return Ok("Event calendar was successfully Created!");
            else
                return BadRequest("Creating event calendar failed!");
        }

        [HttpGet("Revoke")]
        public IActionResult Revoke()
        {
            var statusCode = _service.RevokeToken();
            if (statusCode)
                return Ok("Revoked successfully");
            else
                return BadRequest("Revoking FAILED!");
        }
    }
}
