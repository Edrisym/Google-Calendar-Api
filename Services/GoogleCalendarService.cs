using CalendarApi.Common;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Newtonsoft.Json.Linq;
using Sample.GoogleCalendarApi.Settings;
using RestSharp;
using Newtonsoft.Json;
using Sample.GoogleCalendarApi.Common.Model;
using Microsoft.Extensions.Options;
using NodaTime.Extensions;
using NodaTime;

namespace Sample.GoogleCalendarApi.Services
{
    public class GoogleCalendarService : IGoogleCalendarService
    {
        private readonly GoogleCalendarSettings _settings;
        private readonly IOAuthService _oAuthService;
        private const string _ScopeToken = "https://oauth2.googleapis.com/token";
        private const string _TokenPath = "OAuthFiles/Token.json";
        private const string _CredentialsPath = "OAuthFiles/Credentials.json";

        public GoogleCalendarService(IOptionsSnapshot<GoogleCalendarSettings> settings, IOAuthService oAuthService)
        {
            _settings = settings.Value;
            _oAuthService = oAuthService;
        }

        private Event Create(EventModel model)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");
            var time = DateTimeZone.Utc;
            var eventAttendees = new List<EventAttendee>();

            foreach (var attendee in model.Attendees)
            {
                var eventAttendee = new EventAttendee
                {
                    Email = attendee.AttendeesEmail,
                    DisplayName = attendee.AttendeesName
                };
                eventAttendees.Add(eventAttendee);
            }

            var createdEvent = new Event()
            {
                Summary = model.Summary,
                Location = model.Location,
                CreatedRaw = model.StartDateTime.ToString("yyyy-MM-ddThh:mm:ss.ffffffzzz"),
                // CreatedDateTimeOffset = ,
                // CreatedRaw = "2024-05-08T18:30:0003:30:00Z",
                Description = model.Description,
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

        public async Task<Event> CreateEvent(EventModel model)
        {
            var newEvent = Create(model);

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
            catch (Exception ex)
            {
                throw new Exception("Create Service Account Calendar Failed", ex);
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
            if (response.IsSuccessStatusCode == true)
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
            // Read the JSON file
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
            // var credentialsFile = "/Users/edrisym/Desktop/webApp/File/Credentials.json";
            var credentials = JObject.Parse(System.IO.File.ReadAllText(_CredentialsPath));
            //https://accounts.google.com/o/oauth2/auth?client_id={client_id}&response_type=token&redirect_uri={redirect_uri}&scope={scope}
            try
            {
                string scopeURL1 = "https://accounts.google.com/o/oauth2/auth?redirect_uri={0}&state={1}&response_type={2}&client_id={3}&scope={4}&access_type={5}&include_granted_scopes={6}&login_hint={7}";
                var redirectURL = "https://localhost:7086/oauth/callback";
                string response_type = "code";
                var client_id = credentials["client_id"];
                string scope = "https://www.googleapis.com/auth/calendar+https://www.googleapis.com/auth/calendar.events";
                string access_type = "offline";
                var state = "successful";
                var include_granted_scopes = "true";
                // var prompt = "select_account";
                var login_hint = _settings.LoginHint;
                string redirect_uri_encode = CalendarApi.Common.Method.UrlEncodeForGoogle(redirectURL);
                var mainURL = string.Format(scopeURL1, redirect_uri_encode, state, response_type, client_id, scope, access_type, include_granted_scopes, login_hint);

                return mainURL;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public bool RevokeToken()
        {

            var token = JObject.Parse(File.ReadAllText(_TokenPath));

            // var restClient = new RestClient();
            var request = new RestRequest();

            request.AddQueryParameter("token", token["access_token"].ToString());

            var restClient = new RestClient("https://oauth2.googleapis.com/revoke");
            var response = restClient.ExecutePost(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var newtoken = JObject.Parse(System.IO.File.ReadAllText(_TokenPath));
                Console.WriteLine("successfully revoked the token = {0}", newtoken);
                // return RedirectToAction("Index", "Home", new { status = "success" });
            }

            return response.IsSuccessStatusCode;
        }

        public bool GetToken(string code)
        {
            var credentials = JObject.Parse(File.ReadAllText(_CredentialsPath));

            // string tokenFile = "/Users/edrisym/Desktop/webApp/File/token.json";

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
                if (newTokens.HasValues)
                {
                    UpdateAppSettingJson(newTokens["refresh_token"].ToString());
                }

                Console.WriteLine("StatusCode is OK!");
                Console.WriteLine("request was successfully sent!");
                File.WriteAllText(_TokenPath, response.Content);
            }

            return response.IsSuccessful;
        }

    }
}
