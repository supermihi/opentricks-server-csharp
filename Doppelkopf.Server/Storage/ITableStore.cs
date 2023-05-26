using Doppelkopf.Persistence;
using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.Storage;

public interface ITableStore
{
  Task<TableData?> TryGet(TableId id);
  Task StoreMeta(TableMeta meta);
  IAsyncEnumerable<TableMeta> GetAll();

  Task RecordAction<T>(TableId id, TableAction<T> action)
      where T : ITableActionPayload;
}
