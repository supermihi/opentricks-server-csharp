using System.Text.Json;
using Doppelkopf.API;
using Doppelkopf.Server.TableActions;

namespace Doppelkopf.Server.Notifications;

internal sealed class UserNotificationStreamHandler
{
  private readonly UserId _user;
  private readonly HttpResponse _response;
  private readonly ILogger? _logger;
  private readonly TaskCompletionSource _tcs;

  public UserNotificationStreamHandler(UserId user,
    HttpResponse response,
    ILoggerFactory? loggerFactory)
  {
    _user = user;
    _response = response;
    _logger = loggerFactory?.CreateLogger<UserNotificationStreamHandler>();
    _tcs = new TaskCompletionSource();
    response.OnCompleted(
      () =>
      {
        _tcs.TrySetResult();
        return Task.CompletedTask;
      });
  }

  public Task Ended => _tcs.Task;

  public async Task Send(TableActionResult result, CancellationToken cancellationToken)
  {
    var notification = result.ToNotification(_user);
    await JsonSerializer.SerializeAsync(_response.Body, notification, cancellationToken: cancellationToken);
    await _response.Body.FlushAsync(cancellationToken);
    _logger?.LogInformation("message sent to {User}", _user);
  }

  public void Cancel()
  {
    _tcs.TrySetResult();
  }
}
