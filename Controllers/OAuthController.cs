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

