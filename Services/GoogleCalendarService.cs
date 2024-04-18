using CalendarApi.Common;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util;
using System.IO;
using Newtonsoft.Json.Linq;
using Sample.GoogleCalendarApi.Settings;
using RestSharp;

namespace Sample.GoogleCalendarApi.Services
{
    public class GoogleCalendarService : IGoogleCalendarService
    {
        private readonly IGoogleCalendarSettings _settings;
        private const string refreshToken = "https://oauth2.googleapis.com/token";

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
            var clientId = _settings.ClientId;
            var clientSecre = _settings.ClientSecret;
            var scopes = _settings.Scope;

            //var  = new
            //{
            //    CalendarService.Scope.Calendar,
            //    CalendarService.Scope.CalendarEvents
            //};



            var secrets = new ClientSecrets()
            {
                ClientId = clientId,
                ClientSecret = clientSecre
            };


            // RefreshAccessToken(clientId, clientSecre, scopes);

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



        public bool RefreshAccessToken(string clientId, string clientSecret, string[] scopes)
        {
            string tokenFile = "/Users/edrisym/Desktop/webApp/File/token.json";
            string CredentialsFile = "/Users/edrisym/Desktop/webApp/File/Credentials.json";
            var credentials = JObject.Parse(System.IO.File.ReadAllText(CredentialsFile));
            var token = JObject.Parse(System.IO.File.ReadAllText(tokenFile));

            var restClient = new RestClient();
            var request = new RestRequest();

            request.AddQueryParameter("client_id", credentials["client_id"].ToString());
            request.AddQueryParameter("client_secret", credentials["client_secret"].ToString());
            request.AddQueryParameter("grant_type", "refresh_token");
            request.AddQueryParameter("refresh_token", token["refresh_token"].ToString());

            restClient = new RestClient("https://oauth2.googleapis.com/token");

            var response = restClient.ExecutePost(request);
            if (response.IsSuccessStatusCode == true)
            {
                var newTokens = JObject.Parse(response.Content);
                newTokens["refresh_token"] = token["refresh_token"].ToString();
                Console.WriteLine($"Refresh Token was successfully Made!{newTokens["refresh_token"].ToString()}");

                File.WriteAllText(tokenFile, newTokens.ToString());

            }
            return response.IsSuccessStatusCode;

        }



        public string GetAuthCode()
        {
            var credentialsFile = "/Users/edrisym/Desktop/webApp/File/Credentials.json";
            var credentials = JObject.Parse(System.IO.File.ReadAllText(credentialsFile));
            //https://accounts.google.com/o/oauth2/auth?client_id={client_id}&response_type=token&redirect_uri={redirect_uri}&scope={scope}
            try
            {
                string scopeURL1 = "https://accounts.google.com/o/oauth2/auth?redirect_uri={0}&state={1}&response_type={2}&client_id={3}&scope={4}&access_type={5}&include_granted_scopes={6}&prompt={7}";
                var redirectURL = "https://localhost:7086/oauth/callback";
                string response_type = "code";
                var client_id = credentials["client_id"];
                string scope = "https://www.googleapis.com/auth/calendar+https://www.googleapis.com/auth/calendar.events";
                string access_type = "offline";
                var state = "successful";
                var include_granted_scopes = "true";
                var prompt = "select_account";
                string redirect_uri_encode = CalendarApi.Common.Method.UrlEncodeForGoogle(redirectURL);
                var mainURL = string.Format(scopeURL1, redirect_uri_encode, state, response_type, client_id, scope, access_type, include_granted_scopes, prompt);

                return mainURL;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public bool RevokeToken()
        {
            string tokenFile = "/Users/edrisym/Desktop/webApp/File/token.json";
            var token = JObject.Parse(File.ReadAllText(tokenFile));

            var restClient = new RestClient();
            var request = new RestRequest();

            request.AddQueryParameter("token", token["access_token"].ToString());

            restClient = new RestClient("https://oauth2.googleapis.com/revoke");
            var response = restClient.ExecutePost(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var newtoken = JObject.Parse(System.IO.File.ReadAllText(tokenFile));
                System.Console.WriteLine("successfully revoked the token = {0}", newtoken);
                // return RedirectToAction("Index", "Home", new { status = "success" });
            }

            return response.IsSuccessStatusCode;
        }

        public bool GetToken(string code)
        {
            string CredentialsFile = "/Users/edrisym/Desktop/webApp/File/Credentials.json";
            var credentials = JObject.Parse(System.IO.File.ReadAllText(CredentialsFile));

            string tokenFile = "/Users/edrisym/Desktop/webApp/File/token.json";

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

            restClient = new RestClient(refreshToken);

            var response = restClient.ExecutePost(request);
            Console.WriteLine("request was successfully sent!");

            if (response.IsSuccessful == true)
            {
                System.Console.WriteLine("StatusCode is OK!");
                System.IO.File.WriteAllText(tokenFile, response.Content);

                return response.IsSuccessful;
            }
            return response.IsSuccessful;
        }
    }
}
