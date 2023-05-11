using Doppelkopf.Cards;
using Doppelkopf.Errors;
using Doppelkopf.Persistence;
using Doppelkopf.Tables;

namespace Doppelkopf.Server;

public class PersistingAndPublishingTable : ITable
{
  private VersionedTable? _table;
  private readonly ITableStore _store;
  private readonly ITableActionListener? _listener;
  public TableMeta Meta { get; private set; }

  public PersistingAndPublishingTable(
    TableMeta meta,
    VersionedTable? table,
    ITableStore store,
    ITableActionListener? listener = null
  )
  {
    Meta = meta;
    _store = store;
    _listener = listener;
  }

  private async Task NotifyListener<T>(TableAction<T> action)
    where T : ITableActionPayload
  {
    if (_listener != null)
    {
      await _listener.OnAction(action);
    }
  }

  private (VersionedTable, Seat) TableAndSeat(UserId user)
  {
    if (_table == null)
    {
      throw Err.Table.NotInitialized;
    }
    var seat = Meta.GetSeat(user);
    if (seat == null)
    {
      throw Error.UserNotAtTable;
    }
    return (_table, seat.Value);
  }

  private async Task PersistAndPublish<T>(VersionedTable table, TableAction<T> action)
    where T : ITableActionPayload
  {
    // may fail – in this case, neither update table nor notify listeners
    await _store.RecordAction(Meta.Id, action);
    _table = table;
    await NotifyListener(action);
  }

  public Task PlayCard(UserId user, Card card)
  {
    var (table, seat) = TableAndSeat(user);
    var (nextTable, action) = table.PlayCard(seat, card);
    return PersistAndPublish(nextTable, action);
  }

  public Task Reserve(UserId user, bool reserved)
  {
    var (table, seat) = TableAndSeat(user);
    var (nextTable, action) = table.Reserve(seat, reserved);
    return PersistAndPublish(nextTable, action);
  }
}
