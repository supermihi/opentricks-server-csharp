using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.Notifications;

public interface IClientNotificationStreamHandler
{
  Task Add(UserId user, HttpResponse response);
}