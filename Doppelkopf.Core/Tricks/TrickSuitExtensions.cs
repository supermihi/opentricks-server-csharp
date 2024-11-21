namespace Doppelkopf.Core.Tricks;

public static class TrickSuitExtensions

{
  public static TrickSuit AsTrickSuit(this Suit suit) =>
      suit switch
      {
        Suit.Diamonds => TrickSuit.Diamonds,
        Suit.Hearts => TrickSuit.Hearts,
        Suit.Spades => TrickSuit.Spades,
        Suit.Clubs => TrickSuit.Clubs,
        _ => throw new ArgumentOutOfRangeException(nameof(suit), suit, null)
      };
}
