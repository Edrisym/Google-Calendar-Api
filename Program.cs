var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<GoogleCalendarSettings>(builder.Configuration.GetSection(nameof(GoogleCalendarSettings)));
//builder.Services.AddScoped<IGoogleCalendarSettings, GoogleCalendarSettings>();
builder.Services.AddTransient<IGoogleCalendarService, GoogleCalendarService>();
builder.Services.AddTransient<IOAuthService, OAuthService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
}); 
builder.Services.AddControllers().AddNewtonsoftJson();

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
