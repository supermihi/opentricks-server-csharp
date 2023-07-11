using System.Collections.Immutable;

namespace Doppelkopf.Cards;

public static class Suits
{
    public static readonly ImmutableArray<Suit> InOrder = ImmutableArray.Create(
        Suit.Diamonds,
        Suit.Hearts,
        Suit.Spades,
        Suit.Clubs
    );
}
