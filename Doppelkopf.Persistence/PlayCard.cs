using Doppelkopf.Cards;
using Doppelkopf.Tables;

namespace Doppelkopf.Persistence;

public record PlayCard(Seat Seat, Card Card) : ISeatedTableAction;
