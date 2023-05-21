using System.Collections.Concurrent;
using System.Threading.Channels;
using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.Notifications;

public class NotificationManager
  : BackgroundService,
    IClientNotificationStreamHandler,
    INotificationDispatcher
{
  private readonly ILoggerFactory? _loggerFactory;
  private readonly ILogger<NotificationManager>? _logger;

  public NotificationManager(ILoggerFactory? loggerFactory = null) {
    _loggerFactory = loggerFactory;
    _logger = loggerFactory?.CreateLogger<NotificationManager>();
  }

  private readonly Channel<(IUserNotification, UserId)> _queue = Channel.CreateBounded<(
    IUserNotification,
    UserId
  )>(1_000);
  private readonly ConcurrentDictionary<UserId, UserNotificationStreamHandler> _webSockets = new();

  public Task Add(UserId user, HttpResponse response)
  {
    var handler = _webSockets.AddOrUpdate(
      user,
      addValueFactory: _ => new UserNotificationStreamHandler(user, response, _loggerFactory),
      updateValueFactory: (_, oldWebSocketHandler) =>
      {
        oldWebSocketHandler.Cancel();
        return new UserNotificationStreamHandler(user, response, _loggerFactory);
      }
    );
    return handler.Ended;
  }

  public Task Send(IUserNotification notification, UserId user)
  {
    _queue.Writer.TryWrite((notification, user));
    return Task.CompletedTask;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    await foreach (var (notification, user) in _queue.Reader.ReadAllAsync(stoppingToken))
    {
      if (!_webSockets.TryGetValue(user, out var handler))
      {
        _logger?.LogInformation(
          "Dropping notification {Notification} for offline user {User}",
          notification,
          user
        );
        continue;
      }
      await handler.Send(notification, stoppingToken);
    }
    Console.WriteLine("finish!");
  }
}
