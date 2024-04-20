using Newtonsoft.Json.Linq;
using Sample.GoogleCalendarApi.Common.Model;
namespace Sample.GoogleCalendarApi.Services;

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
}