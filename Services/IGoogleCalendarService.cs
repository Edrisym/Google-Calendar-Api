using System.Diagnostics.Eventing.Reader;
using Google.Apis.Calendar.v3.Data;

namespace Sample.GoogleCalendarApi.Services
{
    public interface IGoogleCalendarService
    {
        Task<Event> CreateEvent();
        string GetAuthCode();
        bool RevokeToken();
        bool RefreshAccessToken(string clientId, string clientSecret, string scopes);

        bool GetToken(string token);
    }
}
