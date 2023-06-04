using Doppelkopf.API;
using Doppelkopf.Server.Interface;
using Doppelkopf.Server.TableActions;

namespace Doppelkopf.Server.Notifications;

public record UserNotification(TableState TableState, IReadOnlyList<TableEvent> Events, DateTime Timestamp)
{
  public static UserNotification FromTableActionResult(TableActionResult result, UserId maskFor)
  {
    return new(result.Table.ToJsonTable(maskFor), result.Events, result.Timestamp);
  }
}
