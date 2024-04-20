using Newtonsoft.Json.Linq;
using Sample.GoogleCalendarApi.Common.Model;
namespace Sample.GoogleCalendarApi.Services;

public interface IOAuthService
{
    JObject CredentialsFile();
    JObject TokenFile();
}
