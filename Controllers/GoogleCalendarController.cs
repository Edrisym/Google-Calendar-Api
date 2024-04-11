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
        [Route("/GoogleCalendar/GetOauthCode")]
        public ActionResult GetOauthCode()
        {
            var get = _googleCalendarService.GetAuthCode();
            return Redirect(get);
        }

        [HttpPost]
        [Route("/GoogleCalendar/CreateEvent")]
        public async Task<IActionResult> CreateEvent()
        {
            return Ok(await _googleCalendarService.CreateEvent());
        }

        [HttpGet]
        [Route("/GoogleCalendar/Callback")]
        public void Callback(string code, string error, string state)
        {
            // if (string.IsNullOrWhiteSpace(error))
            //     this.GetTokens(code);
        }
    }
}
