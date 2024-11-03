namespace GoogleCalendarApi.Services;

public interface IGoogleCalendarService
{
    Task<Event> CreateEventAsync(EventModel model);
    Task<Event?> UpdateEventAsync(string eventId, EventModel eventModel);
    string GetAuthCode();
    Task<bool> RevokeTokenAsync();
    Task<bool> RefreshAccessTokenAsync();
    Task<bool> GetTokenAsync(string token);
}