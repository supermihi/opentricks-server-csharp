using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Tricks;

namespace Doppelkopf.Configuration;

public class QueenSolo : IGameMode
{
  public QueenSolo(EldersMode elders)
  {
    var queens = Suits.InOrder.Select(s => new Card(s, Rank.Queen)).ToImmutableList();
    TrickRules = new TrickRules(queens, elders);
  }

  public GameModeKind Kind => GameModeKind.Solo;

  public ITrickRules TrickRules { get; }
}
