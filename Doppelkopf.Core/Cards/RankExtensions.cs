namespace Doppelkopf.Core.Cards;

public static class RankExtensions
{
  public static int DefaultSideSuitRank(this Rank rank) =>
      rank switch
      {
        Rank.Nine => 1,
        Rank.Jack => 2,
        Rank.Queen => 3,
        Rank.King => 4,
        Rank.Ten => 5,
        Rank.Ace => 6,
        _ => throw new ArgumentOutOfRangeException(nameof(rank), rank, null)
      };
}
