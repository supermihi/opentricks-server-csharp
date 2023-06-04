using Doppelkopf.Server.Authentication;
using Doppelkopf.Server.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Doppelkopf.Server.Controllers;

[ApiController]
[Authorize]
public class NotificationStreamController : ControllerBase
{
  private readonly IClientNotificationStreamHandler _streamHandler;
  private readonly INotificationDispatcher _dispatcher;
  private readonly ILogger<NotificationStreamController>? _logger;

  public NotificationStreamController(IClientNotificationStreamHandler streamHandler,
    INotificationDispatcher dispatcher,
    ILogger<NotificationStreamController>? logger = null)
  {
    _streamHandler = streamHandler;
    _dispatcher = dispatcher;
    _logger = logger;
  }

  [Route("/updates")]
  [HttpGet]
  public async Task GetAsync(CancellationToken cancellationToken)
  {
    cancellationToken.Register(() => _logger?.LogInformation("clinet disconnect"));
    Response.ContentType = "text/event-stream";
    await Response.Body.FlushAsync(cancellationToken);
    await Response.WriteAsync("test", cancellationToken: cancellationToken);
    await _streamHandler.AddStream(HttpContext.AuthenticatedUser().Id, Response);
    _logger?.LogInformation("/updates finished");
  }
}
