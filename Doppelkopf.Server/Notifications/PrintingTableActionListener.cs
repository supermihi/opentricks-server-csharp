using Doppelkopf.Persistence;
using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.Notifications;

public class PrintingTableActionListener : ITableActionListener
{
  public Task OnAction<T>(
      TableData beforeAction,
      TableAction<T> action,
      TableData afterAction
  )
      where T : ITableActionPayload
  {
    Console.WriteLine(action);
    return Task.CompletedTask;
  }
}