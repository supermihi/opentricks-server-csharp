using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Table;

namespace Doppelkopf.Actions;

public interface IEvent { }

public record StartGameEvent(ByPlayer<IImmutableList<Card>> Cards, ByPlayer<Seat> Players)
  : IEvent;

public record PlayCardEvent(Player Player, Card Card) : IEvent;

public record FinishTrickEvent(Player Winner) : IEvent;