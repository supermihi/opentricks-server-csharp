using Doppelkopf.API;
using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.Storage;

public interface ITableStore
{
  Task<Table?> TryGet(TableId id);

  IAsyncEnumerable<Table> GetAll(UserId user);

  Task Create(Table table);
  Task Update(Table previous, Table table);
}
