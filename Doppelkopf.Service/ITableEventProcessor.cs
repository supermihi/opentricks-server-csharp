namespace Doppelkopf.Service;

public interface ITableEventProcessor
{
  Task ProcessTableEvent(TableEvent tableEvent);
}
