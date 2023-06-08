using System.Collections.Concurrent;
using System.Threading.Channels;
using Doppelkopf.Server.TableActions;

namespace Doppelkopf.Server.Notifications;

public class HttpStreamingNotificationHandler
    : BackgroundService,
      IClientNotificationStreamHandler,
      INotificationHandler
{
  private readonly ILoggerFactory? _loggerFactory;
  private readonly ILogger<HttpStreamingNotificationHandler>? _logger;

  public HttpStreamingNotificationHandler(NotificationDispatcher dispatcher, ILoggerFactory? loggerFactory = null)
  {
    _loggerFactory = loggerFactory;
    _unsubscribe = dispatcher.Subscribe(this);
    _logger = loggerFactory?.CreateLogger<HttpStreamingNotificationHandler>();
  }

  private readonly Channel<(TableActionResult result, UserId receiver)> _queue =
      Channel.CreateBounded<(TableActionResult, UserId)>(1_000);
  private readonly ConcurrentDictionary<UserId, UserNotificationStreamHandler> _webSockets = new();
  private readonly IDisposable _unsubscribe;

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

  public async Task OnTableAction(TableActionResult result)
  {
    var receivers = result.Table.Users.Where(u => u != result.Events.First().Actor);
    foreach (var receiver in receivers)
    {
      await _queue.Writer.WriteAsync((result, receiver));
    }
  }

  public Task OnTableAction(TableActionResult result, UserId receiver)
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
          result.Events.First().Type,
          user
        );
        continue;
      }
      await handler.Send(result, stoppingToken);
    }
    Console.WriteLine("finish!");
    _unsubscribe.Dispose();
  }
}
