using Doppelkopf.Cards;
using Doppelkopf.Conf;
using Doppelkopf.Persistence;
using Doppelkopf.Server.Notifications;
using Doppelkopf.Server.Storage;

namespace Doppelkopf.Server.Model;

public class PersistingAndPublishingTable : ITable
{
  public TableData Data { get; private set; }
  private readonly ITableStore _store;
  private readonly ITableActionListener? _listener;
  private readonly Random? _random;

  public PersistingAndPublishingTable(TableData data,
    ITableStore store,
    ITableActionListener? listener = null,
    Random? random = null)
  {
    Data = data;
    _store = store;
    _listener = listener;
    _random = random;
  }

  public async Task AddUser(UserId user)
  {
    Data = Data.AddUser(user, _random);
  }

  private async Task NotifyListener<T>(TableData previous, TableData current, TableAction<T> action)
      where T : ITableActionPayload
  {
    if (_listener != null)
    {
      await _listener.OnAction(previous, action, current);
    }
  }

  private async Task PersistAndPublish<T>(VersionedSession session, TableAction<T> action)
      where T : ITableActionPayload
  {
    // may fail – in this case, neither update table nor notify listeners
    await _store.RecordAction(Data.Meta.Id, action);
    var previous = Data;
    Data = Data with { Table = session };
    await NotifyListener(previous, Data, action);
  }

  public Task Start()
  {
    if (Data.IsStarted)
    {
      throw new ArgumentException("table already started");
    }
    Data = Data with { Table = new(new InitTable(Configuration.DDKV, 4)) };
    return Task.CompletedTask;
  }

  public Task PlayCard(UserId user, Card card)
  {
    var (table, seat) = Data.TableAndSeat(user);
    var (nextTable, action) = table.PlayCard(seat, card);
    return PersistAndPublish(nextTable, action);
  }

  public Task Reserve(UserId user, bool reserved)
  {
    var (table, seat) = Data.TableAndSeat(user);
    var (nextTable, action) = table.Reserve(seat, reserved);
    return PersistAndPublish(nextTable, action);
  }
}
