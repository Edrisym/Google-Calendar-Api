using GoogleCalendarApi.Common;

namespace GoogleCalendarApi.Services;

public class GoogleCalendarService : IGoogleCalendarService
{
    private readonly IOptionsSnapshot<GoogleCalendarSettings> _settings;
    private readonly IOAuthService _oAuthService;
    private const string _ScopeToken = "https://oauth2.googleapis.com/token";
    private const string _TokenPath = "OAuthFiles/Token.json";
    private const string _CredentialsPath = "OAuthFiles/Credentials.json";
    private const string jsonFilePath = "appsettings.Development.json";

    public GoogleCalendarService(IOptionsSnapshot<GoogleCalendarSettings> settings, IOAuthService oAuthService)
    {
        //TODO -- Value
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

    public async Task<Event> CreateEventAsync(EventModel model)
    {
        try
        {
            var newEvent = MakeAnEvent(model);
            //TODO -- to asynchronous   
            var service = _oAuthService.GetCalendarService(_settings.Value);

            var eventRequest = service.Events.Insert(newEvent, _settings.Value.CalendarId);

            eventRequest.SendNotifications = true;
            eventRequest.ConferenceDataVersion = 1;

            var createdEvent = await eventRequest.ExecuteAsync();
            createdEvent.GuestsCanModify = false;

            return createdEvent;
        }
        catch (Exception ex)
        {
            throw new Exception($"Create service account calendar failed !! {ex}");
        }
    }


    private async Task<Event?> GetEventByIdAsync(string eventId)
    {
        var service = _oAuthService.GetCalendarService(_settings.Value);

        var events = await service.Events.List(_settings.Value.CalendarId).ExecuteAsync();

        return events.Items.FirstOrDefault(x => x.Id == eventId);
    }

    public async Task<Event?> UpdateEventAsync(string eventId, EventModel eventModel)
    {
        var eventFound = await GetEventByIdAsync(eventId);
        if (eventFound is null)
        {
            return null;
        }

        var madeEvent = MakeAnEvent(eventModel);

        try
        {
            var service = _oAuthService.GetCalendarService(_settings.Value);

            var request = service.Events.Update(madeEvent, _settings.Value.CalendarId, eventId);
            request.SendNotifications = true;

            var eventMade = await request.ExecuteAsync();
            return eventMade;
        }
        catch (Exception exception)
        {
            throw new ApplicationException($"Updating calendar event failed!! {exception}");
        }
    }

    public async Task<bool> RefreshAccessTokenAsync()
    {
        var credentialFile = await _oAuthService.CredentialsFileAsync();
        var tokenFile = await _oAuthService.TokenFileAsync();

        var request = new RestRequest();

        request.AddQueryParameter("client_id", credentialFile["client_id"]!.ToString());
        request.AddQueryParameter("client_secret", credentialFile["client_secret"]!.ToString());
        request.AddQueryParameter("grant_type", "refresh_token");
        request.AddQueryParameter("refresh_token", tokenFile["refresh_token"]!.ToString());

        var restClient = new RestClient(_ScopeToken);

        var response = await restClient.ExecutePostAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var newTokens = JObject.Parse(response.Content!);
            newTokens["refresh_token"] = tokenFile["refresh_token"]!.ToString();

            await UpdateAppSettingJsonAsync(newTokens["refresh_token"]!.ToString());

            await File.WriteAllTextAsync(_TokenPath, newTokens.ToString());
        }

        return response.IsSuccessStatusCode;
    }

    private async Task UpdateAppSettingJsonAsync(string refreshToken)
    {
        var jsonString = await File.ReadAllTextAsync(jsonFilePath);

        dynamic jsonObj = JsonConvert.DeserializeObject(jsonString)!;

        jsonObj["GoogleCalendarSettings"]["RefreshToken"] = refreshToken;

        string updatedJsonString = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);

        await File.WriteAllTextAsync(jsonFilePath, updatedJsonString);
    }


    public string GetAuthCode()
    {
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
            string redirect_uri_encode = redirectURL.UrlEncodeForGoogle();
            return string.Format(scopeURL1, redirect_uri_encode, state, response_type, client_id, scope,
                access_type, include_granted_scopes, login_hint);
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }

    public async Task<bool> RevokeTokenAsync()
    {
        var token = JObject.Parse(await File.ReadAllTextAsync(Path.GetFullPath(_TokenPath)));
        var request = new RestRequest();

        request.AddQueryParameter("token", token["access_token"].ToString());

        var restClient = new RestClient("https://oauth2.googleapis.com/revoke");
        var response = await restClient.ExecutePostAsync(request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> GetTokenAsync(string code)
    {
        var credentials = JObject.Parse(await File.ReadAllTextAsync(_CredentialsPath));

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

        var response = await restClient.ExecutePostAsync(request);

        //TODO
        if (response.IsSuccessful)
        {
            await File.WriteAllTextAsync(_TokenPath, response.Content);
        }

        return response.IsSuccessful;
    }
}