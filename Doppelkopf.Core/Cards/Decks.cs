using System.Collections.Immutable;

namespace Doppelkopf.Core.Cards;

public static class Decks
{
  public static readonly IImmutableList<Card> WithNines = Card.All.Concat(Card.All).ToImmutableArray();

  public static readonly IImmutableList<Card> WithoutNines = WithNines
    .Where(c => c.Rank != Rank.Nine)
    .ToImmutableArray();

  public static CardsByPlayer Shuffle(this Random random, IEnumerable<Card> deck)
  {
    var shuffledCards = deck.OrderBy(_ => random.Next()).ToImmutableArray();
    var cardsByPlayer = shuffledCards.Length / Rules.NumPlayers;
    return new CardsByPlayer(
      shuffledCards[..cardsByPlayer],
      shuffledCards[cardsByPlayer..(cardsByPlayer * 2)],
      shuffledCards[(cardsByPlayer * 2)..(cardsByPlayer * 3)],
      shuffledCards[(cardsByPlayer * 3)..(cardsByPlayer * 4)]
    );
  }
}
