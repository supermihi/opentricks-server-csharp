using System.Collections.Immutable;

namespace Doppelkopf.Core.Cards;

public static class Suits
{
  public static readonly ImmutableArray<Suit> InOrder = [Suit.Diamonds, Suit.Hearts, Suit.Spades, Suit.Clubs
];
}
