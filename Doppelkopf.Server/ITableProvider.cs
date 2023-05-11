namespace Doppelkopf.Server;

public interface ITableProvider
{
  IAsyncEnumerable<ITable> GetAll();
  Task<ITable> Get(TableId id);
  Task<ITable> Create(UserId owner);
}