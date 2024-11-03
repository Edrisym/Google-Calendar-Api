var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


builder.Services.Configure<GoogleCalendarSettings>(builder.Configuration.GetSection(nameof(GoogleCalendarSettings)));
//TODO
builder.Services.AddTransient<IGoogleCalendarService, GoogleCalendarService>();
builder.Services.AddTransient<IOAuthService, OAuthService>();

var app = builder.Build();



app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseAuthorization();

// app.MapPost(@"test/route", (EventModel model, IGoogleCalendarService _service) =>
// {
//     var createdEvent = _service.CreateEvent(model);
//     return createdEvent;
// });
app.MapControllers();

app.Run();