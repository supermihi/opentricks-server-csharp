using Doppelkopf.Persistence;
using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.Notifications;

public static class TableActionExtensions
{
  public static IUserNotification ToTableUpdateFor<T>(this TableAction<T> action,
    TableData data,
    UserId user)
      where T : ITableActionPayload
  {
    switch (action.Payload)
    {
      case PlayCard pc:
        return new PlayCardUpdate(pc.Card, pc.Seat);
    }
    throw new NotImplementedException();
  }
}
