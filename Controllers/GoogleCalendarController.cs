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
        public async Task<IActionResult> CreateEvent(EventModel model)
        {
            var createdEvent = await _service.CreateEvent(model);
            if (createdEvent != null)
                return Ok("Event calendar was successfully Created!");
            else
                return BadRequest("Creating event calendar failed!");
        }

        [HttpGet("Revoke")]
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



// {
//   "Description": "<p>this is the description</p>\n",
//   "summary": "this is the title",
//   "Location": "edris address",
//   "startDateTime": "2024-05-06T20:20:00",
//   "endDateTime": "2024-05-06T21:21:00",
//   "Attendees": [
//     {
//       "AttendeesEmail": "hafkhat.76@gmail.com",
//       "AttendeesName": "ادریس معاون"
//     }
//   ]
// }



//https://medium.com/iceapple-tech-talks/integration-with-google-calendar-api-using-service-account-1471e6e102c8#:~:text=Enable%20the%20Google%20Calendar%20API.&text=Create%20Credentials%20for%20Service%20account.&text=Add%20%E2%80%9CService%20Account%20Token%20Creator,and%20create%20the%20service%20account.&text=Share%20the%20calendar%20with%20service%20account.