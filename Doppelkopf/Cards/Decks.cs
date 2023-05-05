using System.Collections.Immutable;

namespace Doppelkopf.Cards;

public static class Decks
{
  public static readonly IImmutableList<Card> WithNines = Enum.GetValues<Suit>()
    .SelectMany(
      suit => Enum.GetValues<Rank>().SelectMany(rank => Enumerable.Repeat(new Card(suit, rank), 2))
    )
    .ToImmutableArray();

  public static readonly IImmutableList<Card> WithoutNines = WithNines
    .Where(c => c.Rank != Rank.Nine)
    .ToImmutableArray();

  public static ByPlayer<IImmutableList<Card>> Shuffle(this IEnumerable<Card> deck, Random random)
  {
    var shuffledCards = deck.OrderBy(c => random.Next()).ToImmutableArray();
    var cardsByPlayer = shuffledCards.Length / Constants.NumberOfPlayers;
    return new(
      shuffledCards[..cardsByPlayer].ToImmutableList(),
      shuffledCards[cardsByPlayer..(cardsByPlayer * 2)].ToImmutableList(),
      shuffledCards[(cardsByPlayer * 2)..(cardsByPlayer * 3)].ToImmutableList(),
      shuffledCards[(cardsByPlayer * 3)..(cardsByPlayer * 4)].ToImmutableList()
    );
  }
}
