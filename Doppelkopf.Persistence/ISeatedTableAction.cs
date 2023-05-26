using Doppelkopf.Sessions;

namespace Doppelkopf.Persistence;

public interface ISeatedTableAction : ITableActionPayload
{
  Seat Seat { get; }
}
