namespace Doppelkopf.Cards;

public record Card(Suit Suit, Rank Rank)
{
  public static readonly Card TenOfHearts = new(Suit.Hearts, Rank.Ten);
  public static readonly Card QueenOfClubs = new(Suit.Clubs, Rank.Queen);
}
