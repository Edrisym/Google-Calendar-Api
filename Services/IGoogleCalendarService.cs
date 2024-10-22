﻿
using Google.Apis.Calendar.v3.Data;
using GoogleCalendarApi.Common.Model;

namespace GoogleCalendarApi.Services
{
    public interface IGoogleCalendarService
    {
        Event CreateEvent(EventModel model); 
        Event? UpdateEvent(string eventId,EventModel eventModel);
        string GetAuthCode();
        bool RevokeToken();
        bool RefreshAccessToken(/*string clientId, string clientSecret, string scopes*/);
        bool GetToken(string token);
    }
}
