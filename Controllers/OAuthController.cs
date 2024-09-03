using GoogleCalendarApi.Services;
using GoogleCalendarApi.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GoogleCalendarApi.Controllers
{
    [ApiController]
    public class OAuthController : ControllerBase
    {
        private readonly IGoogleCalendarService _service;
        private readonly GoogleCalendarSettings _settings;
        private const string _ScopeToken = "https://oauth2.googleapis.com/token";
        public OAuthController(IOptionsSnapshot<GoogleCalendarSettings> settings, IGoogleCalendarService service)
        {
            _settings = settings.Value;
            _service  = service;
        }
        [HttpGet]
        [Route("oauth/callback")]
        public IActionResult Callback(string code, string? error, string state)
        {
            if (string.IsNullOrWhiteSpace(error))
                if (_service.GetToken(code))
                    return Ok("Access token was generated successfully!");
                else
                    return BadRequest("Access token failed!!");
            return Ok("Callback method failed");
        }

        [HttpPost]
        [Route("/googlecalendar/generaterefreshtoken")]
        public IActionResult GenerateRefreshToken()
        {

            var status = _service.RefreshAccessToken(/*_settings.ClientId, _settings.ClientSecret, _ScopeToken*/);
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

