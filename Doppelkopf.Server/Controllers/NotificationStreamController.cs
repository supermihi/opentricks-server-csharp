using System.Text.Json.Serialization;
using Doppelkopf.Server.Authentication;
using Doppelkopf.Server.Model;
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

  public NotificationStreamController(
    IClientNotificationStreamHandler streamHandler,
    INotificationDispatcher dispatcher,
    ILogger<NotificationStreamController>? logger = null
  )
  {
    _streamHandler = streamHandler;
    _dispatcher = dispatcher;
    _logger = logger;
  }

  [Route("/updates")]
  public async Task Get(CancellationToken cancellationToken)
  {
    cancellationToken.Register(() => _logger?.LogInformation("clinet disconnect"));
    Response.ContentType = "text/event-stream";
    await Response.Body.FlushAsync(cancellationToken);
    await Response.WriteAsync("test", cancellationToken: cancellationToken);
    await _streamHandler.Add(HttpContext.AuthenticatedUserId(), Response);
    _logger?.LogInformation("/updates finished");
  }

  public record ChatNotification([property: JsonPropertyName("message")] string Message)
    : IUserNotification;

  public record ChatRequest(string Target, string Message);

  [Route("/chat")]
  [HttpPost]
  public async Task Chat([FromForm] string target, [FromForm] string message)
  {
    //var (target, message) = request;
    _logger?.LogInformation(
      "{User} sends message {Message} to {Target}",
      HttpContext.AuthenticatedUserId(),
      message,
      target
    );
    await _dispatcher.Send(new ChatNotification(message), new UserId(target));
  }
}
