var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


builder.Services.Configure<GoogleCalendarSettings>(builder.Configuration.GetSection(nameof(GoogleCalendarSettings)));

builder.Services.AddScoped<IGoogleCalendarService, GoogleCalendarService>();
builder.Services.AddScoped<IOAuthService, OAuthService>();

var app = builder.Build();
app.UseRouting();
app.MapControllers();

app.Run();