using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.Notifications;

public interface IClientNotificationStreamHandler
{
  Task AddStream(UserId user, HttpResponse response);
}
