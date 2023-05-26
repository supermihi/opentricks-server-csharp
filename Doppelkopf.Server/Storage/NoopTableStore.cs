using Doppelkopf.Persistence;
using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.Storage;

class NoopTableStore : ITableStore
{
  public Task<TableData?> TryGet(TableId id)
  {
    return Task.FromResult<TableData?>(null);
  }

  public Task StoreMeta(TableMeta meta)
  {
    return Task.CompletedTask;
  }

  public async IAsyncEnumerable<TableMeta> GetAll()
  {
    yield break;
  }

  public Task RecordAction<T>(TableId id, TableAction<T> action)
      where T : ITableActionPayload
  {
    return Task.CompletedTask;
  }
}
