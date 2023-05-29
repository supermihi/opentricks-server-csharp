using Doppelkopf.Server.TableActions;

namespace Doppelkopf.Server.Notifications;

public interface ITableActionListener
{
  Task Notify(TableActionResult actionResult);
}
