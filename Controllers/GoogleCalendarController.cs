namespace GoogleCalendarApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GoogleCalendarController : ControllerBase
{
    private readonly IGoogleCalendarService _service;
    private const string MessagePattern = "Event created for the calendar {0}";

    public GoogleCalendarController(IGoogleCalendarService service)
    {
        _service = service;
    }

    [HttpPost("/")]
    public async Task<string> CreateEvent([FromQuery] EventModel model)
    {
        var createdEvent = await _service.CreateEventAsync(model);
        return string.Format(MessagePattern, createdEvent.Id);
    }

    [HttpGet("/Revoke")]
    public async Task<bool> Revoke()
    {
        return await _service.RevokeTokenAsync();
    }

    [HttpPut("/{eventId}")]
    public async Task<Event?> UpdateEvent(string eventId, [FromQuery] EventModel eventModel)
    {
        return await _service.UpdateEventAsync(eventId, eventModel);
    }
}