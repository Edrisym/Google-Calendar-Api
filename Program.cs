using Microsoft.Extensions.Options;
using Sample.GoogleCalendarApi.Services;
using Sample.GoogleCalendarApi.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<GoogleCalendarSettings>(builder.Configuration.GetSection(nameof(GoogleCalendarSettings)));
builder.Services.AddSingleton<IGoogleCalendarSettings>(s => s.GetRequiredService<IOptions<GoogleCalendarSettings>>().Value);

builder.Services.AddScoped<IGoogleCalendarService, GoogleCalendarService>();
builder.Services.AddScoped<IOAuthService, OAuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
