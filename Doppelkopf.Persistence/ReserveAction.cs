using Doppelkopf.Tables;

namespace Doppelkopf.Persistence;

public record ReserveAction(Seat Seat, bool Reserved, int Version) : ISeatedTableAction;