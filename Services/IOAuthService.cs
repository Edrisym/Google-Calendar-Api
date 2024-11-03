using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using GoogleCalendarApi.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace GoogleCalendarApi.Services;

public interface IOAuthService
{
    Task<JObject>  CredentialsFileAsync();
    Task<JObject>  TokenFileAsync();
    CalendarService GetCalendarService(IOptionsSnapshot<GoogleCalendarSettings> calendarSetting);
}