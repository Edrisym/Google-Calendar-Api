using CalendarApi.Common;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Newtonsoft.Json.Linq;
using Sample.GoogleCalendarApi.Settings;

namespace Sample.GoogleCalendarApi.Services
{
    public class GoogleCalendarService : IGoogleCalendarService
    {
        private readonly IGoogleCalendarSettings _settings;

        public GoogleCalendarService(IGoogleCalendarSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        ///  Create Google Calendar Event base on out request
        /// </summary>
        /// <param name="request">Google Event model where we write information about out Event</param>
        /// <param name="cancellationToken">Google Event model where we write information about out Event</param>
        /// <returns>Google Event</returns>
        public async Task<Event> CreateEvent()
        {
            var newEvent = new Event()
            {
                Summary = "this is an invitation",
                Location = "tehran",
                Description = "this is edris ghafouri",
                Start = new EventDateTime()
                {
                    DateTime = DateTime.Now,
                    TimeZone = "Asia/Tehran",
                },
                End = new EventDateTime()
                {
                    DateTime = DateTime.Now,
                    TimeZone = "Asia/Tehran",
                },
                Attendees = new EventAttendee[] {
                    new() { Email = "hafkhat.76@gmail.com" },
                    new() { Email = "edrismaven@gmail.com" }
                },
                Reminders = new Event.RemindersData()
                {
                    UseDefault = false,
                    Overrides = new EventReminder[] {
                        new() { Method = "email", Minutes = 24 * 60 },
                        new() { Method = "sms", Minutes = 10 },
                    }
                }
            };

            var secrets = new ClientSecrets()
            {
                ClientId = _settings.ClientId,
                ClientSecret = _settings.ClientSecret
            };
            var token = new TokenResponse { RefreshToken = _settings.RefreshToken };


            var credential = new UserCredential(new GoogleAuthorizationCodeFlow(
              new GoogleAuthorizationCodeFlow.Initializer
              {
                  ClientSecrets = secrets
              }),
                _settings.User,
                token);

            // define services
            var services = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = _settings.ApplicationName,
            });

            EventsResource.InsertRequest eventRequest = services.Events.Insert(newEvent, _settings.CalendarId);
            try
            {
                eventRequest.SendNotifications = true;
                Event createdEvent = eventRequest.Execute();
                Console.WriteLine("Event created: {0}", createdEvent.HtmlLink);
                return createdEvent;
            }
            catch (System.Exception ex)
            {
                throw new Exception("Create Service Account Calendar Failed", ex);
            }
        }

        public string GetAuthCode()
        {
            var credentialsFile = "/Users/edrisym/Desktop/webApp/File/Credentials.json";
            var credentials = JObject.Parse(System.IO.File.ReadAllText(credentialsFile));
            ///https://accounts.google.com/o/oauth2/auth?client_id={client_id}&response_type=token&redirect_uri={redirect_uri}&scope={scope}
            try
            {
                string scopeURL1 = "https://accounts.google.com/o/oauth2/auth?redirect_uri={0}&state={1}&response_type={2}&client_id={3}&scope={4}&access_type={5}&include_granted_scopes={6}";
                var redirectURL = "https://localhost:7086/googlecalendar/callback";
                string response_type = "code";
                var client_id = credentials["client_id"];
                string scope = "https://www.googleapis.com/auth/calendar+https://www.googleapis.com/auth/calendar.events";
                string access_type = "offline";
                var state = "successful";
                var include_granted_scopes = "true";
                string redirect_uri_encode = Method.UrlEncodeForGoogle(redirectURL);
                var mainURL = string.Format(scopeURL1, redirect_uri_encode, state, response_type, client_id, scope, access_type, include_granted_scopes);

                return mainURL;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

    }
}
