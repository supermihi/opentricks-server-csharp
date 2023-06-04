using Doppelkopf.API;
using Doppelkopf.Server.TableActions;

namespace Doppelkopf.Server.Notifications;

public interface INotificationDispatcher
{
  Task Send(TableActionResult result, UserId receiver);
}
