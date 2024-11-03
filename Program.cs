var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.RegisterSettingService();
builder.Services.AddScoped<IGoogleCalendarService, GoogleCalendarService>();
builder.Services.AddScoped<IOAuthService, OAuthService>();

var app = builder.Build();
app.UseRouting();
app.MapControllers();

app.Run();


public static class AddSetting
{
    public static void RegisterSettingService(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<GoogleCalendarSettings>
            (builder.Configuration.GetSection(nameof(GoogleCalendarSettings)));
    }
}