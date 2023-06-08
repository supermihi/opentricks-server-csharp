using Doppelkopf.API;
using Doppelkopf.Server.Authentication;
using Doppelkopf.Server.Bots;
using Doppelkopf.Server.Controllers;
using Doppelkopf.Server.Notifications;
using Doppelkopf.Server.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options => JsonConfiguration.SetupJsonOptions(options.JsonSerializerOptions));
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddSingleton<ITableStore, InMemoryTableStore>()
    .AddSingleton<NotificationDispatcher>()
    .AddSingleton<HttpStreamingNotificationHandler>()
    .AddSingleton<IClientNotificationStreamHandler>(s => s.GetRequiredService<HttpStreamingNotificationHandler>())
    .AddTransient<ILoginHandler, AllowAllLoginHandler>()
    .AddTransient<ITableService, TableService>()
    .AddSingleton(BotIds.Default)
    .AddHostedService(s => s.GetRequiredService<HttpStreamingNotificationHandler>())
    .AddHostedService<BotService>()
    .AddAuthentication()
    .AddScheme<DebugAuthenticationOptions, DebugAuthenticationHandler>(
      DebugAuthenticationOptions.Schema,
      _ => { }
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseWebSockets(new() { KeepAliveInterval = TimeSpan.FromSeconds(2) });
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
