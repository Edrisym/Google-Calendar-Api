using System.Net;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using GoogleCalendarApi.Settings;


namespace GoogleCalendarApi.Services;

public class OAuthService : IOAuthService
{
    private const string CredentialFilePath = "OAuthFiles/Credentials.json";
    private const string TokenFilePath = "OAuthFiles/Token.json";

    public async Task<JObject> CredentialsFileAsync()
    {
        return JObject.Parse(await File.ReadAllTextAsync(CredentialFilePath));
    }

    public async Task<JObject> TokenFileAsync()
    {
        return JObject.Parse(await File.ReadAllTextAsync(TokenFilePath));
    }


    public CalendarService GetCalendarService(IOptionsSnapshot<GoogleCalendarSettings> calendarSetting)
    {
        var secrets = new ClientSecrets
        {
            ClientId = calendarSetting.Value.ClientId,
            ClientSecret = calendarSetting.Value.ClientSecret
        };

        var token = new TokenResponse { RefreshToken = calendarSetting.Value.RefreshToken };

        var flow = new GoogleAuthorizationCodeFlow(
            new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = secrets
            });

        var credential = new UserCredential(flow, calendarSetting.Value.User, token);

        var services = new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = calendarSetting.Value.ApplicationName,
        });
        return services;
    }
}