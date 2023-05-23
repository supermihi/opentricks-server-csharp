using Doppelkopf.Persistence;
using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.Notifications;

public interface ITableActionListener
{
    Task OnAction<T>(TableData beforeAction, TableAction<T> action, TableData afterAction)
      where T : ITableActionPayload;
}