using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
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
builder.Services
    .AddAuthentication(o =>
 {
     var defaultAuthorizationBuilder = new AuthorizationPolicyBuilder("Bearer");
     // This forces challenge results to be handled by Google OpenID Handler, so there's no
     // need to add an AccountController that emits challenges for Login.
     o.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
     // This forces forbid results to be handled by Google OpenID Handler, which checks if
     // extra scopes are required and does automatic incremental auth.
     o.DefaultForbidScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
     // Default scheme that will handle everything else.
     // Once a user is authenticated, the OAuth2 token info is stored in cookies.
     o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
 })
        .AddCookie()
        .AddGoogleOpenIdConnect(options =>
        {
            options.ClientId = "626680043851-rpjifvhg68ks612kv650886noaq9tr23.apps.googleusercontent.com";
            options.ClientSecret = "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";
        });

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
