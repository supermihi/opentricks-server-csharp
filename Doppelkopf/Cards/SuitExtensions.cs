namespace Doppelkopf.Cards;

public static class SuitExtensions
{
  public static string Display(this Suit suit) =>
      suit switch
      {
        Suit.Diamonds => "♦",
        Suit.Hearts => "♥",
        Suit.Spades => "♠",
        Suit.Clubs => "♣",
        _ => throw new ArgumentOutOfRangeException(nameof(suit), suit, null)
      };
}
