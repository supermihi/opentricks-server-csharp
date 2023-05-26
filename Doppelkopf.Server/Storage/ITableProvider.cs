using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.Storage;

public interface ITableProvider
{
  IAsyncEnumerable<ITable> GetAll();
  Task<ITable> Get(TableId id);
  Task<ITable> Create(UserId owner, string name);
}
