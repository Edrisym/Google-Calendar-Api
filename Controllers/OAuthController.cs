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

        public OAuthController(IGoogleCalendarService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("oauth/callback")]
        public async Task<bool> Callback(string code, string? error, string state)
        {
            if (string.IsNullOrWhiteSpace(error))
                return await _service.GetTokenAsync(code);
            return false;
        }

        [HttpPost]
        [Route("/refreshToken")]
        public async Task<bool> GenerateRefreshToken()
        {
            return await _service.RefreshAccessTokenAsync();
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