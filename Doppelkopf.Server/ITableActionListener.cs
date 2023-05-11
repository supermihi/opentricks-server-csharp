using Doppelkopf.Persistence;

namespace Doppelkopf.Server;

public interface ITableActionListener
{
  Task OnAction<T>(TableAction<T> action)
    where T : ITableActionPayload;
}

class PrintingTableActionListener : ITableActionListener
{
  public Task OnAction<T>(TableAction<T> action)
    where T : ITableActionPayload
  {
    Console.WriteLine(action);
    return Task.CompletedTask;
  }
}
