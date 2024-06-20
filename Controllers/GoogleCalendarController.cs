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
            var eventId = createdEvent.Id;
            if (eventId != null)
                return Ok($"Creating event calendar was successfully created : {eventId}");
            return BadRequest("Creating event calendar failed!");
        }

        [HttpGet("Revoke")]
        public IActionResult Revoke()
        {
            var statusCode = _service.RevokeToken();
            if (statusCode)
                return Ok("Revoking was successfully created");
            else
                return BadRequest("Revoking failed!");
        }

        [HttpPut("UpdateEvent{eventId}")]
        public IActionResult UpdateEvent(string eventId, [FromBody] EventModel eventModel)
        {
            var createdEvent = _service.UpdateEvent(eventId, eventModel);
            if (createdEvent is null)
            {
                return NotFound("Event with this Id was not found !");
            }

            return Ok(createdEvent);
        }
    }
}