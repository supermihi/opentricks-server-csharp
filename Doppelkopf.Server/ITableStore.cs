using System.Collections.Concurrent;
using Doppelkopf.Persistence;

namespace Doppelkopf.Server;

public interface ITableStore
{
  Task<(TableMeta, VersionedTable?)?> TryGet(TableId id);
  Task StoreMeta(TableMeta meta);
  IAsyncEnumerable<TableMeta> GetAll();
  Task RecordAction<T>(TableId id, TableAction<T> action)
    where T : ITableActionPayload;
}

class NoopTableStore : ITableStore
{
  public Task<(TableMeta, VersionedTable?)?> TryGet(TableId id)
  {
    return Task.FromResult<(TableMeta, VersionedTable?)?>(null);
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
