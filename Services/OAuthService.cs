using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using GoogleCalendarApi.Settings;


namespace GoogleCalendarApi.Services;

public class OAuthService : IOAuthService
{
    public JObject CredentialsFile()
    {
        string CredentialsFile = "OAuthFiles/Credentials.json";
        return JObject.Parse(File.ReadAllText(CredentialsFile));
    }

    public JObject TokenFile()
    {
        string tokenFile = "OAuthFiles/Token.json";
        return JObject.Parse(File.ReadAllText(tokenFile));
    }

    public CalendarService GetCalendarService(IOptionsSnapshot<GoogleCalendarSettings> calendarSetting)
    {
       var secrets = new ClientSecrets()
       {
           ClientId = calendarSetting.Value.ClientId,
           ClientSecret = calendarSetting.Value.ClientSecret
       };
      
       var token = new TokenResponse { RefreshToken = calendarSetting.Value.RefreshToken };
      
       var credential = new UserCredential(new GoogleAuthorizationCodeFlow(
         new GoogleAuthorizationCodeFlow.Initializer
         {
             ClientSecrets = secrets
         }),
           calendarSetting.Value.User,
           token);
      
       var services = new CalendarService(new BaseClientService.Initializer()
       {
           HttpClientInitializer = credential,
           ApplicationName = calendarSetting.Value.ApplicationName,
       });
       return services;
    }
}