using System.Collections.Immutable;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Cards;

public static class Decks
{
    public static readonly IImmutableList<Card> WithNines = Enum.GetValues<Suit>()
        .SelectMany(suit => Enum.GetValues<Rank>().Select(rank => new Card(suit, rank)))
        .SelectMany(card => new[] { card, card })
        .ToImmutableArray();

    public static readonly IImmutableList<Card> WithoutNines = WithNines
        .Where(c => c.Rank != Rank.Nine)
        .ToImmutableArray();

    public static CardsByPlayer Shuffle(this IEnumerable<Card> deck, Random random)
    {
        var shuffledCards = deck.OrderBy(_ => random.Next()).ToImmutableArray();
        var cardsByPlayer = shuffledCards.Length / Rules.NumPlayers;
        return new(
            new ByPlayer<ImmutableArray<Card>>(
                shuffledCards[..cardsByPlayer].ToImmutableArray(),
                shuffledCards[cardsByPlayer..(cardsByPlayer * 2)].ToImmutableArray(),
                shuffledCards[(cardsByPlayer * 2)..(cardsByPlayer * 3)].ToImmutableArray(),
                shuffledCards[(cardsByPlayer * 3)..(cardsByPlayer * 4)].ToImmutableArray()
            )
        );
    }
}
