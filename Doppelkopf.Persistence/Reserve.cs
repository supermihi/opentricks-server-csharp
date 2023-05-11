using Doppelkopf.Tables;

namespace Doppelkopf.Persistence;

public record Reserve(Seat Seat, bool Reserved) : ISeatedTableAction;
