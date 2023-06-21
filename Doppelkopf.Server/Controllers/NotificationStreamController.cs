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
  private readonly ILogger<NotificationStreamController>? _logger;

  public NotificationStreamController(IClientNotificationStreamHandler streamHandler,
    ILogger<NotificationStreamController>? logger = null)
  {
    _streamHandler = streamHandler;
    _logger = logger;
  }

  [Route("/updates")]
  [HttpGet]
  public async Task GetAsync(CancellationToken cancellationToken)
  {
    cancellationToken.Register(() => _logger?.LogInformation("client disconnect"));
    Response.ContentType = "text/event-stream";
    await Response.Body.FlushAsync(cancellationToken);
    await _streamHandler.AddStream(HttpContext.AuthenticatedUser().Id, Response, cancellationToken);
    _logger?.LogInformation("/updates finished");
  }
}
