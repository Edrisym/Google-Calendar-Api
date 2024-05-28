using System.Diagnostics.Eventing.Reader;
using Google.Apis.Calendar.v3.Data;
using Newtonsoft.Json.Linq;
using Sample.GoogleCalendarApi.Common.Model;

namespace Sample.GoogleCalendarApi.Services
{
    public interface IGoogleCalendarService
    {
        Event CreateEvent(EventModel model);
        string GetAuthCode();
        bool RevokeToken();
        bool RefreshAccessToken(/*string clientId, string clientSecret, string scopes*/);
        bool GetToken(string token);
    }
}
