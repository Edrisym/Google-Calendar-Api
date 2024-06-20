using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Newtonsoft.Json.Linq;
using Sample.GoogleCalendarApi.Settings;
using RestSharp;
using Newtonsoft.Json;
using Sample.GoogleCalendarApi.Common.Model;
using Microsoft.Extensions.Options;

namespace Sample.GoogleCalendarApi.Services
{
    public class GoogleCalendarService : IGoogleCalendarService
    {
        private readonly IOptionsSnapshot<GoogleCalendarSettings> _settings;
        private readonly IOAuthService _oAuthService;
        private const string _ScopeToken = "https://oauth2.googleapis.com/token";
        private const string _TokenPath = "OAuthFiles/Token.json";
        private const string _CredentialsPath = "OAuthFiles/Credentials.json";

        public GoogleCalendarService(IOptionsSnapshot<GoogleCalendarSettings> settings, IOAuthService oAuthService)
        {
            _settings = settings;
            _oAuthService = oAuthService;
        }

        private static Event MakeAnEvent(EventModel model)
        {
            var eventAttendees = new List<EventAttendee>();
            foreach (var attendee in model.Attendees)
            {
                var eventAttendee = new EventAttendee
                {
                    Email = attendee.AttendeesEmail,
                    DisplayName = attendee.AttendeesName,
                    Comment = attendee.AlocomHangoutLink
                };
                eventAttendees.Add(eventAttendee);
            }

            var createdEvent = new Event
            {
                Creator = new Event.CreatorData()
                {
                    DisplayName = "شرکت حساب رایان"
                },
                ColorId = "11",
                Description = model.Description,
                Summary = model.Summary,
                Location = model.Location,
                Start = new EventDateTime()
                {
                    DateTimeDateTimeOffset = model.StartDateTime,
                    TimeZone = model.TimeZone
                },
                End = new EventDateTime()
                {
                    DateTimeDateTimeOffset = model.EndDateTime,
                    TimeZone = model.TimeZone,
                },
                Attendees = eventAttendees
            };

            return createdEvent;
        }

        public Event CreateEvent(EventModel model)
        {
            try
            {
                var newEvent = MakeAnEvent(model);
                var service = _oAuthService.GetCalendarService(_settings);

                var eventRequest = service.Events.Insert(newEvent, _settings.Value.CalendarId);

                eventRequest.SendNotifications = true;
                eventRequest.ConferenceDataVersion = 1;

                var createdEvent = eventRequest.Execute();
                createdEvent.GuestsCanModify = false;
                Console.WriteLine($"Event created: {createdEvent.Id}");

                return createdEvent;
            }
            catch (Exception ex)
            {
                throw new Exception($"Create service account calendar failed !! {ex}");
            }
        }


        private Event? getEventById(string eventId)
        {
            var service = _oAuthService
                .GetCalendarService(_settings);
            
            var events = service.Events.List(_settings.Value.CalendarId).Execute();
            
            var eventItem = events.Items
                .FirstOrDefault(x => x.Id == eventId);
            
            return eventItem;
        }

        public Event? UpdateEvent(string eventId, EventModel eventModel)
        {
            var eventFound = getEventById(eventId);
            if (eventFound is null)
            {
                return null;
            }
            
            var madeEvent = MakeAnEvent(eventModel);
            
            try
            {
                var service = _oAuthService
                    .GetCalendarService(_settings);
                
                var request = service.Events
                    .Update(madeEvent, _settings.Value.CalendarId, eventId);
                
                request.SendNotifications = true;
                
                var eventMade = request.Execute();
                return eventMade;
            }
            catch (Exception exception)
            {
                throw new ApplicationException($"Updating calendar event failed!! {exception}");
            }
        }

        public bool RefreshAccessToken()
        {
            var credentialFile = _oAuthService.CredentialsFile();
            var tokenFile = _oAuthService.TokenFile();

            var request = new RestRequest();

            request.AddQueryParameter("client_id", credentialFile["client_id"].ToString());
            request.AddQueryParameter("client_secret", credentialFile["client_secret"].ToString());
            request.AddQueryParameter("grant_type", "refresh_token");
            request.AddQueryParameter("refresh_token", tokenFile["refresh_token"].ToString());

            var restClient = new RestClient(_ScopeToken);

            var response = restClient.ExecutePost(request);
            if (response.IsSuccessStatusCode)
            {
                var newTokens = JObject.Parse(response.Content);
                newTokens["refresh_token"] = tokenFile["refresh_token"].ToString();

                UpdateAppSettingJson(newTokens["refresh_token"].ToString());

                File.WriteAllText(_TokenPath, newTokens.ToString());
            }

            return response.IsSuccessStatusCode;
        }

        public void UpdateAppSettingJson(string refreshToken)
        {
            string jsonFilePath = "appsettings.Development.json";
            string jsonString = File.ReadAllText(jsonFilePath);

            dynamic jsonObj = JsonConvert.DeserializeObject(jsonString);

            jsonObj["GoogleCalendarSettings"]["RefreshToken"] = refreshToken;

            string updatedJsonString = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);

            File.WriteAllText(jsonFilePath, updatedJsonString);

            Console.WriteLine($"RefreshToken updated successfully. {refreshToken}");
        }


        public string GetAuthCode()
        {
            var credentials = JObject.Parse(System.IO.File.ReadAllText(_CredentialsPath));
            try
            {
                string scopeURL1 =
                    "https://accounts.google.com/o/oauth2/auth?redirect_uri={0}&state={1}&response_type={2}&client_id={3}&scope={4}&access_type={5}&include_granted_scopes={6}&login_hint={7}";
                var redirectURL = "https://localhost:7086/oauth/callback";
                string response_type = "code";
                var client_id = _settings.Value.ClientId;
                string scope =
                    "https://www.googleapis.com/auth/calendar+https://www.googleapis.com/auth/calendar.events";
                string access_type = "offline";
                var state = "successful";
                var include_granted_scopes = "true";
                // var prompt = "select_account";
                var login_hint = _settings.Value.LoginHint;
                string redirect_uri_encode = CalendarApi.Common.Method.UrlEncodeForGoogle(redirectURL);
                var mainURL = string.Format(scopeURL1, redirect_uri_encode, state, response_type, client_id, scope,
                    access_type, include_granted_scopes, login_hint);

                return mainURL;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public bool RevokeToken()
        {
            var token = JObject.Parse(File.ReadAllText(Path.GetFullPath(_TokenPath)));
            var request = new RestRequest();

            request.AddQueryParameter("token", token["access_token"].ToString());

            var restClient = new RestClient("https://oauth2.googleapis.com/revoke");
            var response = restClient.ExecutePost(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var newtoken = JObject.Parse(System.IO.File.ReadAllText(_TokenPath));
                Console.WriteLine("successfully revoked the token = {0}", newtoken);
            }

            return response.IsSuccessStatusCode;
        }

        public bool GetToken(string code)
        {
            var credentials = JObject.Parse(File.ReadAllText(_CredentialsPath));

            //TODO
            var restClient = new RestClient();
            var request = new RestRequest();

            request.AddQueryParameter("client_id", credentials["client_id"].ToString());
            request.AddQueryParameter("client_secret", credentials["client_secret"].ToString());
            request.AddQueryParameter("code", code);
            request.AddQueryParameter("access_type", "offline");
            request.AddQueryParameter("grant_type", "authorization_code");
            // request.AddQueryParameter("prompt", "consent");
            request.AddQueryParameter("redirect_uri", "https://localhost:7086/oauth/callback");

            restClient = new RestClient(_ScopeToken);

            var response = restClient.ExecutePost(request);

            //TODO
            if (response.IsSuccessful == true)
            {
                var newTokens = JObject.Parse(response.Content);
                //if (newTokens.HasValues)
                //{
                //    UpdateAppSettingJson(newTokens["refresh_token"].ToString());
                //}

                Console.WriteLine("StatusCode is OK!");
                Console.WriteLine("request was successfully sent!");
                File.WriteAllText(_TokenPath, response.Content);
            }

            return response.IsSuccessful;
        }

        public void GetColor()
        {
            var service = _oAuthService.GetCalendarService(_settings);
            var colorRequest = service.Colors.Get();
            var colors = colorRequest.Execute();

            foreach (var color in colors.Event__)
            {
                Console.WriteLine(
                    $"ColorId: {color.Key}, Background: {color.Value.Background}, Foreground: {color.Value.Foreground}");
            }
        }
    }
}