using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Sample.GoogleCalendarApi.Services;
using Sample.GoogleCalendarApi.Settings;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sample.GoogleCalendarApi.Controllers
{
    [ApiController]
    public class OAuthController : ControllerBase
    {
        private readonly IGoogleCalendarService _googleCalendarService;
        private readonly IGoogleCalendarSettings _googleCalendarSettings;
        public OAuthController(IGoogleCalendarService googleCalendarService, IGoogleCalendarSettings googleCalendarSettings)
        {
            _googleCalendarService = googleCalendarService;
            _googleCalendarSettings = googleCalendarSettings;
        }
        [HttpGet]
        [Route("oauth/callback")]
        public async Task<IActionResult> Callback(string code, string? error, string state)
        {
            if (string.IsNullOrWhiteSpace(error))
                if (_googleCalendarService.GetToken(code))
                    return Ok();
                else
                    return BadRequest();


            return Ok();
        }

        [HttpPost]
        [Route("/googlecalendar/generaterefreshtoken")]
        public async Task<IActionResult> GenerateRefreshToken()
        {
            var scopes = new[] { "https://oauth2.googleapis.com/token" };
            var status = _googleCalendarService.RefreshAccessToken(_googleCalendarSettings.ClientId, _googleCalendarSettings.ClientSecret, scopes);
            if (status)
                return Ok();
            else
                return BadRequest();
        }

        [HttpGet]
        [Route("/oauth/getoauthcode")]
        public IActionResult GetOauthCode()
        {
            var uri = _googleCalendarService.GetAuthCode();
            if (!String.IsNullOrEmpty(uri))
                return Redirect(uri);
            else
                return BadRequest();
        }
    }
}

