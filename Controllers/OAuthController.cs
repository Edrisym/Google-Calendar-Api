using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sample.GoogleCalendarApi.Controllers
{
    public class OAuthController : ControllerBase
    {
        public void Callback(string code, string error, string state)
        {
            if (string.IsNullOrWhiteSpace(error))
                Console.WriteLine("Callback");
            //this.GetTokens(code);
        }
    }
}

