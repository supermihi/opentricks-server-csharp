using System.Collections.Immutable;
using Doppelkopf.Cards;

namespace Doppelkopf.Configuration;

public static class Extensions
{
  public static IImmutableList<Card> Deck(this IRules rules) =>
    rules.RuleSet.WithoutNines ? Decks.WithoutNines : Decks.WithNines;
}
