namespace GoogleCalendarApi.Services;

public interface IOAuthService
{
    Task<JObject> CredentialsFileAsync();
    Task<JObject> TokenFileAsync();
    CalendarService GetCalendarService(IOptionsSnapshot<GoogleCalendarSettings> calendarSetting);
}