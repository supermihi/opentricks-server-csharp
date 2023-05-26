using Doppelkopf.Persistence;
using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.Notifications;

public class UserNotifyingTableActionListener : ITableActionListener
{
  private readonly INotificationDispatcher _notificationDispatcher;

  public UserNotifyingTableActionListener(INotificationDispatcher notificationDispatcher)
  {
    _notificationDispatcher = notificationDispatcher;
  }

  public async Task OnAction<T>(TableData previous, TableAction<T> action, TableData data)
      where T : ITableActionPayload
  {
    await Task.WhenAll(
      data.Users.Select(
        user =>
        {
          var notification = action.ToTableUpdateFor(data, user);
          return _notificationDispatcher.Send(notification, user);
        })
    );
  }
}
