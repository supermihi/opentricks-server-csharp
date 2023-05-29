using Doppelkopf.Server.TableActions;

namespace Doppelkopf.Server.Notifications;

public class UserNotifyingTableActionListener : ITableActionListener
{
  private readonly INotificationDispatcher _notificationDispatcher;

  public UserNotifyingTableActionListener(INotificationDispatcher notificationDispatcher)
  {
    _notificationDispatcher = notificationDispatcher;
  }

  public async Task Notify(TableActionResult actionResult)
  {
    await Task.WhenAll(
      actionResult.Table.Users.Select(
        user => _notificationDispatcher.Send(actionResult, user))
    );
  }
}
