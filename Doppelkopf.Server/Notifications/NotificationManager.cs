using System.Collections.Concurrent;
using System.Threading.Channels;
using Doppelkopf.API;
using Doppelkopf.Server.TableActions;

namespace Doppelkopf.Server.Notifications;

public class NotificationManager
    : BackgroundService,
      IClientNotificationStreamHandler,
      INotificationDispatcher
{
  private readonly ILoggerFactory? _loggerFactory;
  private readonly ILogger<NotificationManager>? _logger;

  public NotificationManager(ILoggerFactory? loggerFactory = null)
  {
    _loggerFactory = loggerFactory;
    _logger = loggerFactory?.CreateLogger<NotificationManager>();
  }

  private readonly Channel<(TableActionResult result, UserId receiver)> _queue =
      Channel.CreateBounded<(TableActionResult, UserId)>(1_000);
  private readonly ConcurrentDictionary<UserId, UserNotificationStreamHandler> _webSockets = new();

  public Task AddStream(UserId user, HttpResponse response)
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

  public Task Send(TableActionResult result, UserId receiver)
  {
    _queue.Writer.TryWrite((result, receiver));
    return Task.CompletedTask;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    await foreach (var (result, user) in _queue.Reader.ReadAllAsync(stoppingToken))
    {
      if (!_webSockets.TryGetValue(user, out var handler))
      {
        _logger?.LogInformation(
          "Dropping action result {ActionResult} for offline user {User}",
          result,
          user
        );
        continue;
      }
      await handler.Send(result, stoppingToken);
    }
    Console.WriteLine("finish!");
  }
}
