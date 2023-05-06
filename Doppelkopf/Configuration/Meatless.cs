using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Tricks;

namespace Doppelkopf.Configuration;

public class MeatFree : IGameMode
{
  public GameModeKind Kind => GameModeKind.Solo;

  public ITrickRules TrickRules { get; } =
    new TrickRules(
      ImmutableList<Card>.Empty,
      EldersMode.FirstWins /* irrelevant, no trump! */
    );
}
