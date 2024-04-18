using Microsoft.AspNetCore.Mvc;
using Sample.GoogleCalendarApi.Services;
using Sample.GoogleCalendarApi.Settings;

namespace Sample.GoogleCalendarApi.Controllers
{
    [ApiController]
    public class OAuthController : ControllerBase
    {
        private readonly IGoogleCalendarService _service;
        private readonly IGoogleCalendarSettings _settings;
        public OAuthController(IGoogleCalendarService service, IGoogleCalendarSettings settings)
        {
            _service = service;
            _settings = settings;
        }
        [HttpGet]
        [Route("oauth/callback")]
        public async Task<IActionResult> Callback(string code, string? error, string state)
        {
            if (string.IsNullOrWhiteSpace(error))
                if (_service.GetToken(code))
                    return Ok();
                else
                    return BadRequest();


            return Ok();
        }

        [HttpPost]
        [Route("/googlecalendar/generaterefreshtoken")]
        public async Task<IActionResult> GenerateRefreshToken()
        {

            var status = _service.RefreshAccessToken(_settings.ClientId, _settings.ClientSecret, _settings.ScopeToken);
            if (status)
                return Ok();
            else
                return BadRequest();
        }

        [HttpGet]
        [Route("/oauth/getoauthcode")]
        public IActionResult GetOauthCode()
        {
            var uri = _service.GetAuthCode();
            if (!String.IsNullOrEmpty(uri))
                return Redirect(uri);
            else
                return BadRequest();
        }
    }
}

