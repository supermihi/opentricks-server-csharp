namespace Doppelkopf.Core.Scoring;

public static class CardExtensions
{
  public static int Points(this Card card) =>
      card.Rank switch
      {
        Rank.Nine => 0,
        Rank.Jack => 2,
        Rank.Queen => 3,
        Rank.King => 4,
        Rank.Ten => 10,
        Rank.Ace => 11,
        _ => throw new ArgumentOutOfRangeException(nameof(card))
      };
}
