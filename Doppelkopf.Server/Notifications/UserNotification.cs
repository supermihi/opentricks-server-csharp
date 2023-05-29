using Doppelkopf.Server.Interface;
using Doppelkopf.Server.Model;
using Doppelkopf.Server.TableActions;

namespace Doppelkopf.Server.Notifications;

public record UserNotification(JsonTable Table, IReadOnlyList<TableEvent> Events, DateTime Timestamp)
{
  public static UserNotification FromTableActionResult(TableActionResult result, UserId maskFor)
  {
    return new(JsonTable.FromTable(result.Table, maskFor), result.Events, result.Timestamp);
  }
}
