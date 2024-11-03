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


    public CalendarService GetCalendarService(GoogleCalendarSettings calendarSetting)
    {
        var secrets = new ClientSecrets
        {
            ClientId = calendarSetting.ClientId,
            ClientSecret = calendarSetting.ClientSecret
        };

        var token = new TokenResponse { RefreshToken = calendarSetting.RefreshToken };

        var flow = new GoogleAuthorizationCodeFlow(
            new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = secrets
            });


        var credential = new UserCredential(flow, calendarSetting.User, token);

        var services = new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = calendarSetting.ApplicationName,
        });
        return services;
    }
}