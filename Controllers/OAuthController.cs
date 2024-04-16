using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sample.GoogleCalendarApi.Controllers
{
    [ApiController]
    public class OAuthController : ControllerBase
    {
        [HttpGet]
        [Route("oauth/callback")]
        public async Task<IActionResult> Callback(string code, string error, string state)
        {
            if (string.IsNullOrWhiteSpace(error))
                Console.WriteLine("Callback");
            return Ok();
        }
    }
}

