namespace Doppelkopf.Service;

public interface ITableStore
{
  Task CreateTable(ITable table);
  Task<ITable> GetTable(TableId id);
}
