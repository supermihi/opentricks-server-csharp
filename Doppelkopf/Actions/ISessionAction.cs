using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Utils;

namespace Doppelkopf.Actions;

public interface ISessionAction
{
  Player? Player { get; }
}

public sealed record StartGameAtion(ByPlayer<IImmutableList<Card>> DealtCards, Player Leader) : ISessionAction
{
  public Player? Player => null;
}
