using Doppelkopf.API;

namespace Doppelkopf.Server.Notifications;

public interface IClientNotificationStreamHandler
{
  Task AddStream(UserId user, HttpResponse response, CancellationToken cancellationToken);
}
