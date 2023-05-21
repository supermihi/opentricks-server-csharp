using Doppelkopf.Cards;
using Doppelkopf.Sessions;

namespace Doppelkopf.Persistence;

public record PlayCard(Seat Seat, Card Card) : ISeatedTableAction;
