using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Tricks;

namespace Doppelkopf.Configuration;

public class JackSolo : IGameMode
{
  public JackSolo(EldersMode elders)
  {
    TrickRules = new TrickRules(
      Suits.InOrder.Select(s => new Card(s, Rank.Jack)).ToImmutableList(),
      elders
    );
  }

  public GameModeKind Kind => GameModeKind.Solo;

  public ITrickRules TrickRules { get; }
}
