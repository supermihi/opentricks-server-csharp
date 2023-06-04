using System.Collections.Concurrent;
using Doppelkopf.API;
using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.Storage;

class InMemoryTableStore : ITableStore
{
  private readonly ConcurrentDictionary<TableId, Table> _tables = new();
  public Task<Table?> TryGet(TableId id)
  {
    return Task.FromResult(_tables.TryGetValue(id, out var result) ? result : null);
  }

  public async IAsyncEnumerable<Table> GetAll(UserId user)
  {
    await Task.Yield();
    foreach (var value in _tables.Values.Where(t => t.Users.Contains(user)))
    {
      yield return value;
    }
  }

  public Task Create(Table table)
  {
    _tables[table.Meta.Id] = table;
    return Task.CompletedTask;
  }

  public Task Update(Table previous, Table next)
  {
    _tables.TryUpdate(next.Meta.Id, next, previous);
    return Task.CompletedTask;
  }
}
