namespace Doppelkopf.Service;

internal sealed class DoppelkopfService
{
  private readonly ITableFactory _tableFactory;
  private readonly ITableStore _store;

  public DoppelkopfService(ITableFactory tableFactory, ITableStore store)
  {
    _tableFactory = tableFactory;
    _store = store;
  }
  public async Task<TableId> CreateTable(UserId creator, string tableName, int maxSeats)
  {
    var table = _tableFactory.Create(tableName, creator);
    await _store.CreateTable(table);
    return table.Data.Id;
  }

  public async Task StartTable(UserId user, TableId id)
  {
    var table = await _store.GetTable(id);
    if (table.Data.Owner != user)
    {
      throw ErrorCodes.NotOwner.ToException();
    }
    await table.Start();
  }

  public async Task PlayCard(UserId user, TableId tableId, Card card)
  {
    var table = await _store.GetTable(tableId);
    await table.PlayCard(user, card);
  }
}
