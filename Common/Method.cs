namespace GoogleCalendarApi.Common;

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
        if (windowsTimeZoneId.Equals("UTC", StringComparison.OrdinalIgnoreCase))
            return "Etc/UTC";

        if (windowsTimeZoneId.Equals("Iran Standard Time", StringComparison.OrdinalIgnoreCase))
            return "Asia/Tehran";

        var tzdbSource = TzdbDateTimeZoneSource.Default;
        var windowsMapping = tzdbSource.WindowsMapping.PrimaryMapping
            .FirstOrDefault(mapping => mapping.Key.Equals(windowsTimeZoneId, StringComparison.OrdinalIgnoreCase));

        return windowsMapping.Value;
    }
}