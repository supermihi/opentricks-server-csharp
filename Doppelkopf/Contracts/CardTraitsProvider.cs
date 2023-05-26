using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Tricks;

namespace Doppelkopf.Contracts;

public sealed class CardTraitsProvider : ICardTraitsProvider
{
  private readonly ImmutableArray<Card> _trumpRankedAsc;
  private readonly ImmutableArray<Rank> _sideSuitRankedAsc;

  public CardTraitsProvider(IEnumerable<Card> trumpByValueAsc,
    IEnumerable<Rank>? rankInOrder = null)
  {
    _trumpRankedAsc = trumpByValueAsc.ToImmutableArray();
    _sideSuitRankedAsc = rankInOrder?.ToImmutableArray() ?? DefaultSideSuitRanked;
  }

  public static ImmutableArray<Rank> DefaultSideSuitRanked = ImmutableArray.Create(
    Rank.Nine,
    Rank.Jack,
    Rank.Queen,
    Rank.King,
    Rank.Ten,
    Rank.Ace
  );

  public CardTraits Get(Card card)
  {
    var trumpRank = _trumpRankedAsc.IndexOf(card);
    if (trumpRank != -1)
    {
      return new(TrickSuit.Trump, trumpRank);
    }
    var sideRank = _sideSuitRankedAsc.IndexOf(card.Rank);
    return new(card.Suit.AsTrickSuit(), sideRank);
  }

  public static readonly CardTraitsProvider NoTrump = new(Enumerable.Empty<Card>());

  public static readonly CardTraitsProvider DiamondsTrump = new(SuitSoloTrump(Suit.Diamonds));
  public static readonly CardTraitsProvider HeartsTrump = new(SuitSoloTrump(Suit.Hearts));
  public static readonly CardTraitsProvider SpadesTrump = new(SuitSoloTrump(Suit.Spades));
  public static readonly CardTraitsProvider ClubsTrump = new(SuitSoloTrump(Suit.Clubs));

  public static readonly CardTraitsProvider JacksTrump =
      new(
        new Card[]
        {
            new(Suit.Diamonds, Rank.Jack), new(Suit.Hearts, Rank.Jack), new(Suit.Spades, Rank.Jack),
            new(Suit.Clubs, Rank.Jack)
        }
      );

  public static readonly CardTraitsProvider QueensTrump =
      new(
        new Card[]
        {
            new(Suit.Diamonds, Rank.Queen), new(Suit.Hearts, Rank.Queen), new(Suit.Spades, Rank.Queen),
            new(Suit.Clubs, Rank.Queen)
        }
      );

  private static IEnumerable<Card> SuitSoloTrump(Suit suit)
  {
    return ImmutableList
        .Create<Card>()
        .Add(new Card(suit, Rank.Nine))
        .Add(new Card(suit, Rank.King))
        .AddRange(suit == Suit.Hearts ? Enumerable.Empty<Card>() : new[] { new Card(suit, Rank.Ten) })
        .Add(new(suit, Rank.Ace))
        .AddRange(Suits.InOrder.Select(s => new Card(s, Rank.Jack)))
        .AddRange(Suits.InOrder.Select(s => new Card(s, Rank.Queen)))
        .Add(Card.TenOfHearts);
  }
}
