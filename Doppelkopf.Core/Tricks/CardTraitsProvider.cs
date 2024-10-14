using System.Collections.Immutable;
using Doppelkopf.Core.Cards;

namespace Doppelkopf.Core.Tricks;

internal sealed class CardTraitsProvider(IReadOnlyDictionary<Card, CardTraits> traits) : ICardTraitsProvider
{
  private static readonly ImmutableArray<Card> HigherTrump =
    Card.Jacks.ToImmutableArray().AddRange(Card.Queens).Add(Card.HeartsTen);

  private static readonly ImmutableArray<Rank> SideSuitRanks = [Rank.Nine, Rank.Jack, Rank.Queen, Rank.King, Rank.Ten, Rank.Ace
];

  public static ICardTraitsProvider SuitSolo(Suit trumpSuit, TieBreakingMode heartsTenTieBreaking)
  {
    var lowerTrump = SideSuitRanks
      .Select(r => new Card(trumpSuit, r))
      .Where(card => !HigherTrump.Contains(card));
    return ForTrumpWithDefaultSides(lowerTrump.Concat(HigherTrump), heartsTenTieBreaking);
  }

  public static ICardTraitsProvider NormalGame(TieBreakingMode heartsTenTieBreaking) =>
    SuitSolo(Suit.Diamonds, heartsTenTieBreaking);

  public static CardTraitsProvider ForTrumpWithDefaultSides(IEnumerable<Card> trumpInOrder,
    TieBreakingMode heartsTenTieBreaking = TieBreakingMode.FirstWins)
  {
    var trump = trumpInOrder.ToImmutableArray();
    var traits = new Dictionary<Card, CardTraits>();
    foreach (var card in Card.All)
    {
      if (trump.Contains(card))
      {
        traits[card] = new CardTraits(
          TrickSuit.Trump,
          trump.IndexOf(card),
          card == Card.HeartsTen ? heartsTenTieBreaking : TieBreakingMode.FirstWins);
      }
      else
      {
        traits[card] = new CardTraits(
          card.Suit.AsTrickSuit(),
          card.Rank.DefaultSideSuitRank(),
          TieBreakingMode.FirstWins);
      }
    }

    return new CardTraitsProvider(traits);
  }

  public CardTraits Get(Card card) => traits[card];
}
