namespace Doppelkopf.Cards;

public record Card(Suit Suit, Rank Rank) {
  public static Card TenOfHearts = new(Suit.Hearts, Rank.Ten);
}
