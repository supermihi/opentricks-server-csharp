using Doppelkopf.Cards;

namespace Doppelkopf.Configuration;

public static class Extensions
{
  public static IReadOnlyList<Card> Deck(this IRules rules) =>
    rules.RuleSet.WithoutNines ? Decks.WithoutNines : Decks.WithNines;
}
