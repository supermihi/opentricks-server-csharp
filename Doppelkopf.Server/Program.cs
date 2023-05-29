using Doppelkopf.Server.Authentication;
using Doppelkopf.Server.Interface;
using Doppelkopf.Server.Notifications;
using Doppelkopf.Server.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(JsonConfiguration.SetupJsonOptions);
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddSingleton<ITableStore, InMemoryTableStore>()
    .AddSingleton<ITableActionListener, UserNotifyingTableActionListener>()
    .AddSingleton<NotificationManager>()
    .AddSingleton<INotificationDispatcher>(s => s.GetRequiredService<NotificationManager>())
    .AddSingleton<IClientNotificationStreamHandler>(s => s.GetRequiredService<NotificationManager>())
    .AddScoped<ILoginHandler, AllowAllLoginHandler>()
    .AddHostedService(s => s.GetRequiredService<NotificationManager>())
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
