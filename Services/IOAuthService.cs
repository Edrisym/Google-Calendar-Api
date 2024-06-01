using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Sample.GoogleCalendarApi.Settings;

namespace Sample.GoogleCalendarApi.Services;

public interface IOAuthService
{
    JObject CredentialsFile();
    JObject TokenFile();
    CalendarService GetCalendarService(IOptionsSnapshot<GoogleCalendarSettings> calendarSetting);

}
