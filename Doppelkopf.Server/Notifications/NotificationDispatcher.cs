using Doppelkopf.Server.TableActions;

namespace Doppelkopf.Server.Notifications;

public class NotificationDispatcher
{
  private readonly List<INotificationHandler> _handlers = new();
  private readonly object _handlersLock = new();

  private class HandlerRemover : IDisposable
  {
    private readonly INotificationHandler _handler;
    private readonly NotificationDispatcher _dispatcher;

    internal HandlerRemover(INotificationHandler handler, NotificationDispatcher dispatcher)
    {
      _handler = handler;
      _dispatcher = dispatcher;
    }

    public void Dispose() => _dispatcher._handlers.Remove(_handler);
  }

  public IDisposable Subscribe(INotificationHandler handler)
  {
    lock (_handlersLock)
    {
      _handlers.Add(handler);
      return new HandlerRemover(handler, this);
    }
  }

  public Task Notify(TableActionResult action)
  {
    Task result;
    lock (_handlersLock)
    {
      result = Task.WhenAll(_handlers.Select(h => h.OnTableAction(action)));
    }
    return result;
  }
}

public interface INotificationHandler
{
  Task OnTableAction(TableActionResult result);
}
