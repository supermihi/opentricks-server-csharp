using Doppelkopf.Tables;

namespace Doppelkopf.Persistence;

public interface ISeatedTableAction : ITableAction
{
  Seat Seat { get; }
}