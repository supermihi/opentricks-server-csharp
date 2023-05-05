using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Tricks;

namespace Doppelkopf.Configuration;

public class JackSolo : IGameMode
{
  private static readonly ITrickRules StaticTrickRules = new TrickRules(
    Trump: Suits.InOrder.Select(s => new Card(s, Rank.Jack)).ToImmutableList()
  );
  public GameModeKind Kind => GameModeKind.Solo;

  public ITrickRules TrickRules => StaticTrickRules;
}
