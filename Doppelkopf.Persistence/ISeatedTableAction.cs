using Doppelkopf.Tables;

namespace Doppelkopf.Persistence;

public interface ISeatedTableAction : ITableActionPayload
{
  Seat Seat { get; }
}
