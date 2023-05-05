using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Tricks;

namespace Doppelkopf.Configuration;

public class QueenSolo : IGameMode
{
  public GameModeKind Kind => GameModeKind.Solo;

  public ITrickRules TrickRules { get; } =
    new TrickRules(Suits.InOrder.Select(s => new Card(s, Rank.Queen)).ToImmutableList());
}
