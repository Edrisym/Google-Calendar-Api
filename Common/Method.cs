using System.Text;
using NodaTime.TimeZones;

namespace CalendarApi.Common;

public class Method
{
    public static string UrlEncodeForGoogle(string url)
    {
        var unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-.~";
        var result = new StringBuilder();
        foreach (var symbol in url)
        {
            if (unreservedChars.IndexOf(symbol) != -1)
            {
                result.Append(symbol);
            }
            else
            {
                result.Append("%" + ((int)symbol).ToString("X2"));
            }
        }

        return result.ToString();

    }

    public static string WindowsToIana(string windowsTimeZoneId)
    {
        // Special case for UTC
        if (windowsTimeZoneId.Equals("UTC", StringComparison.OrdinalIgnoreCase))
            return "Etc/UTC";

        // Check if the provided Windows time zone identifier is for Tehran
        if (windowsTimeZoneId.Equals("Iran Standard Time", StringComparison.OrdinalIgnoreCase))
            return "Asia/Tehran";

        // For other Windows time zones, use the Noda Time library to find the mapping
        var tzdbSource = TzdbDateTimeZoneSource.Default;
        var windowsMapping = tzdbSource.WindowsMapping.PrimaryMapping
            .FirstOrDefault(mapping => mapping.Key.Equals(windowsTimeZoneId, StringComparison.OrdinalIgnoreCase));

        return windowsMapping.Value; // Return the corresponding IANA time zone identifier, if found
    }

}
