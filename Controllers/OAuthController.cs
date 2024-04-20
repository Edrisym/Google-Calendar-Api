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
        private const string _ScopeToken = "https://oauth2.googleapis.com/token";
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
                    return Ok("Access token was generated successfully!");
                else
                    return BadRequest("Access token failed!!");
            return Ok("Callbacl method Failed");
        }

        [HttpPost]
        [Route("/googlecalendar/generaterefreshtoken")]
        public async Task<IActionResult> GenerateRefreshToken()
        {

            var status = _service.RefreshAccessToken(_settings.ClientId, _settings.ClientSecret, _ScopeToken);
            if (status)
                return Ok("Refresh token was generated successfully!");
            else
                return BadRequest("Refresh token failed!!");
        }

        [HttpGet]
        [Route("/oauth/getoauthcode")]
        public IActionResult GetOauthCode()
        {
            var uri = _service.GetAuthCode();
            if (!String.IsNullOrEmpty(uri))
                return Redirect(uri);
            else
                return BadRequest("creating Uri redirect was failed!!");
        }
    }
}

