using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Configuration;

namespace Doppelkopf.Tricks;

public readonly record struct TrickRules(IImmutableList<Card> Trump, IImmutableList<Rank> SideSuit)
    : ITrickRules {

  public TrickRules(IImmutableList<Card> Trump) : this(Trump, DefaultSideSuitOrder) { }

  public CardComparison Compare(Card current, Card best) {
    var currentTrumpRank = Trump.IndexOf(current);
    var bestTrumpRank = Trump.IndexOf(best);
    var isSideSuitTrick = bestTrumpRank == -1;
    var currentIsSideSuit = currentTrumpRank == -1;
    return (isSideSuitTrick, currentIsSideSuit) switch {
        (true, true) => CompareSideSuits(current, best),
        (false, false) => CompareTrump(current, best),
        (true, false) => CardComparison.Higher,
        (false, true) => CardComparison.Lower
    };
  }

  public bool SameTrickSuit(Card a, Card b) {
    return GetTrickSuit(a) == GetTrickSuit(b);
  }

  private TrickSuit GetTrickSuit(Card card) {
    var isTrump = Trump.Contains(card);
    return isTrump ? TrickSuit.Trump : card.Suit.AsTrickSuit();
  }

  private CardComparison CompareSideSuits(Card current, Card best) {
    var currentRank = SideSuit.IndexOf(current.Rank);
    var bestRank = SideSuit.IndexOf(best.Rank);
    return currentRank.CompareTo(bestRank).AsCardComparison();
  }

  private CardComparison CompareTrump(Card current, Card best) {
    var currentRank = Trump.IndexOf(current);
    var bestRank = Trump.IndexOf(best);
    return currentRank.CompareTo(bestRank).AsCardComparison();
  }

  public static TrickRules ForTrumpSuit(Suit trump) {
    var deck = Decks.WithNines;
    var lowerTrumpCards = deck.Where(
        card => card.Suit == trump && !HigherTrumpsByValue.Contains(card)
    );
    return new(lowerTrumpCards.Concat(HigherTrumpsByValue).ToImmutableList(), DefaultSideSuitOrder);
  }
  
  private static readonly ImmutableList<Rank> DefaultSideSuitOrder = ImmutableList.Create(
      Rank.Nine,
      Rank.Jack, // only in meat-free and queen solo
      Rank.Queen, // only in meat-free an jack solo
      Rank.King,
      Rank.Ten,
      Rank.Ace
  );

  private static readonly ImmutableList<Card> HigherTrumpsByValue = ImmutableList.Create<Card>(
      new(Suit.Diamonds, Rank.Jack),
      new(Suit.Hearts, Rank.Jack),
      new(Suit.Spades, Rank.Jack),
      new(Suit.Clubs, Rank.Jack),
      new(Suit.Diamonds, Rank.Queen),
      new(Suit.Hearts, Rank.Queen),
      new(Suit.Spades, Rank.Queen),
      new(Suit.Clubs, Rank.Queen),
      new(Suit.Hearts, Rank.Ten)
  );
}