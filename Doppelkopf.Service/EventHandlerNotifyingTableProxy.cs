using Doppelkopf.Core.Cards;

namespace Doppelkopf.Service;

public class EventHandlerNotifyingTableProxy : ITable
{
  private readonly ITableEventProcessor _processor;
  private readonly ITable _table;

  public EventHandlerNotifyingTableProxy(ITableEventProcessor processor, ITable table)
  {
    _processor = processor;
    _table = table;
  }

  public TableData Data => _table.Data;
  public async Task Start()
  {
    await _table.Start();
    var tableEvent = TableEvent.Start(_table);
    await _processor.ProcessTableEvent(tableEvent);
  }

  public async Task<PlayCardResult> PlayCard(UserId user, Card card)
  {
    var result = await _table.PlayCard(user, card);
    var tableEvent = TableEvent.PlayCard(_table, card, user, result);
    await _processor.ProcessTableEvent(tableEvent);
    return result;
  }
}
