using Doppelkopf.Cards;
using Doppelkopf.Tables;

namespace Doppelkopf.Persistence;

public record PlayCardAction(Seat Seat, Card Card, int Version) : ISeatedTableAction;