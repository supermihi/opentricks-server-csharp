using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core;

public sealed record RuleOptions(
  TieBreakingMode HeartTenTieBreaking,
  bool IncludeNines)
{
  public IReadOnlyList<Card> Deck => IncludeNines ? Decks.WithNines : Decks.WithoutNines;
}
