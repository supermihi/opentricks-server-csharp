using System.Collections.Immutable;
using Doppelkopf.Core.Cards;

namespace Doppelkopf.Core.Tricks;

public interface ICardTraitsProvider
{
  CardTraits GetTraits(Card card);
}

public sealed class CardTraitsProvider : ICardTraitsProvider
{
  private readonly IReadOnlyDictionary<Card, CardTraits> _traits;

  public CardTraitsProvider(IReadOnlyDictionary<Card, CardTraits> traits)
  {
    _traits = traits;
  }

  public static ImmutableArray<Card> HigherTrump =
      Card.Jacks.ToImmutableArray().AddRange(Card.Queens).Add(Card.HeartsTen);

  public static ImmutableArray<Rank> SideSuitRanks = ImmutableArray.Create(
    Rank.Nine,
    Rank.Jack,
    Rank.Queen,
    Rank.King,
    Rank.Ten,
    Rank.Ace
  );

  public static CardTraitsProvider SuitSolo(Suit trumpSuit, TieBreakingMode heartsTenTieBreaking)
  {
    var lowerTrump = SideSuitRanks
        .Select(r => new Card(trumpSuit, r))
        .Where(card => !HigherTrump.Contains(card));
    return ForTrumpWithDefaultSides(lowerTrump.Concat(HigherTrump), heartsTenTieBreaking);
  }

  public static CardTraitsProvider ForTrumpWithDefaultSides(IEnumerable<Card> trumpInOrder,
    TieBreakingMode heartsTenTieBreaking = TieBreakingMode.FirstWins)
  {
    var trump = trumpInOrder.ToImmutableArray();
    var traits = new Dictionary<Card, CardTraits>();
    foreach (var card in Card.All)
    {
      if (trump.Contains(card))
      {
        traits[card] = new(
          TrickSuit.Trump,
          trump.IndexOf(card),
          card == Card.HeartsTen ? heartsTenTieBreaking : TieBreakingMode.FirstWins);
      }
      else
      {
        traits[card] = new(card.Suit.AsTrickSuit(), card.Rank.DefaultSideSuitRank(), TieBreakingMode.FirstWins);
      }
    }
    return new(traits);
  }

  public CardTraits GetTraits(Card card) => _traits[card];
}
