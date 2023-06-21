using Doppelkopf.Server.TableActions;

namespace Doppelkopf.Server.Notifications;

public interface INotificationHandler
{
  Task OnTableAction(TableActionResult result, UserId actor);
}
