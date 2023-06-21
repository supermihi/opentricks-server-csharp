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
  private readonly ConcurrentDictionary<UserId, UserNotificationStreamHandler> _handlers = new();
  private readonly IDisposable _unsubscribe;

  public Task AddStream(UserId user, HttpResponse response, CancellationToken cancellationToken)
  {
    var handler = _handlers.AddOrUpdate(
      user,
      addValueFactory: _ => new UserNotificationStreamHandler(user, response, _loggerFactory),
      updateValueFactory: (_, oldHandler) =>
      {
        oldHandler.Cancel();
        return new UserNotificationStreamHandler(user, response, _loggerFactory);
      }
    );
    cancellationToken.Register(handler.Cancel);
    return handler.Ended;
  }

  public async Task OnTableAction(TableActionResult result, UserId actor)
  {
    var receivers = result.Table.Users.Where(u => u != actor);
    foreach (var receiver in receivers)
    {
      await _queue.Writer.WriteAsync((result, receiver));
    }
  }


  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    await foreach (var (result, user) in _queue.Reader.ReadAllAsync(stoppingToken))
    {
      if (!_handlers.TryGetValue(user, out var handler))
      {
        _logger?.LogInformation(
          "Dropping event {Event} for offline user {User}",
          result.Events.Last(),
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
