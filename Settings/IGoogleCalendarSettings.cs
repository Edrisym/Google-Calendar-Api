namespace Sample.GoogleCalendarApi.Settings
{
    public interface IGoogleCalendarSettings
    {
        string ClientId { get; set; }
        string ClientSecret { get; set; }
        string[] Scope { get; set; }
        string ApplicationName { get; set; }
        string User { get; set; }
        string CalendarId { get; }
        string RefreshToken { get; set; }
    }
}
