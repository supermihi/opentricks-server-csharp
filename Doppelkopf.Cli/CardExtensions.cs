using Doppelkopf.Core.Cards;

namespace Doppelkopf.Cli;

public static class CardExtensions
{
  public static string Display(this Rank r) =>
    r switch
    {
      Rank.Nine => "9",
      Rank.Jack => "J",
      Rank.Queen => "Q",
      Rank.King => "K",
      Rank.Ten => "10",
      Rank.Ace => "A",
      _ => throw new ArgumentOutOfRangeException(nameof(r), r, null)
    };

  public static string Display(this Suit s) =>
    s switch
    {
      Suit.Diamonds => "\u2666",
      Suit.Hearts => "\u2665",
      Suit.Spades => "\u2660",
      Suit.Clubs => "\u2663",
      _ => throw new ArgumentOutOfRangeException(nameof(s), s, null)
    };

  public static string Display(this Card c) => $"{c.Suit.Display()}{c.Rank.Display()}";
}
