namespace Doppelkopf.Cards;

public enum Rank
{
  Nine,
  Jack,
  Queen,
  King,
  Ten,
  Ace
}

public static class RankExtensions
{
  public static string Display(this Rank rank) =>
      rank switch
      {
        Rank.Nine => "9",
        Rank.Jack => "J",
        Rank.Queen => "Q",
        Rank.King => "K",
        Rank.Ten => "10",
        Rank.Ace => "A",
        _ => throw new ArgumentOutOfRangeException(nameof(rank), rank, null)
      };
}
