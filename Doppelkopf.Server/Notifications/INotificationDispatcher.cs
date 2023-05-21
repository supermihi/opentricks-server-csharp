using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.Notifications;

public interface INotificationDispatcher
{
  Task Send(IUserNotification notification, UserId user);
}