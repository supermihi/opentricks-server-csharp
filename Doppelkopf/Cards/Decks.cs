using System.Collections.Immutable;

namespace Doppelkopf.Cards;

public static class Decks
{
  public static readonly IImmutableList<Card> WithNines = Enum.GetValues<Suit>()
    .SelectMany(suit => Enum.GetValues<Rank>().Select(rank => new Card(suit, rank)))
    .SelectMany(card => new[] { card, card })
    .ToImmutableArray();

  public static readonly IImmutableList<Card> WithoutNines = WithNines
    .Where(c => c.Rank != Rank.Nine)
    .ToImmutableArray();

  public static ByPlayer<IImmutableList<Card>> Shuffle(this IEnumerable<Card> deck, Random random)
  {
    var shuffledCards = deck.OrderBy(_ => random.Next()).ToImmutableArray();
    var cardsByPlayer = shuffledCards.Length / Constants.NumberOfPlayers;
    return new(
      shuffledCards[..cardsByPlayer].ToImmutableList(),
      shuffledCards[cardsByPlayer..(cardsByPlayer * 2)].ToImmutableList(),
      shuffledCards[(cardsByPlayer * 2)..(cardsByPlayer * 3)].ToImmutableList(),
      shuffledCards[(cardsByPlayer * 3)..(cardsByPlayer * 4)].ToImmutableList()
    );
  }
}
