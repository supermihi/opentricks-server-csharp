namespace Doppelkopf.Service;

public class MemoryTableStore : ITableStore
{
  private readonly Dictionary<TableId, ITable> _data = new();

  public Task CreateTable(ITable table)
  {
    if (_data.ContainsKey(table.Data.Id))
    {
      throw new ArgumentException("table already in store");
    }
    _data[table.Data.Id] = table;
    return Task.CompletedTask;
  }

  public Task<ITable> GetTable(TableId id)
  {
    return Task.FromResult(_data[id]);
  }
}