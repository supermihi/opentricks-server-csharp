namespace Doppelkopf.Cards;

public record struct Card(Suit Suit, Rank Rank)
{
  public static readonly Card TenOfHearts = new(Suit.Hearts, Rank.Ten);
  public static readonly Card QueenOfClubs = new(Suit.Clubs, Rank.Queen);

  public string Id => $"{Suit.Display()}-{Rank.Display()}";
}
