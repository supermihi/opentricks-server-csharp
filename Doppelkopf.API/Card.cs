namespace Doppelkopf.API;

public readonly record struct Card(Suit Suit, Rank Rank)
{
  public static readonly Card HeartsTen = new(Suit.Hearts, Rank.Ten);
  public static readonly Card ClubsQueen = new(Suit.Clubs, Rank.Queen);
  public static IEnumerable<Card> All =>
      Enum.GetValues<Suit>().SelectMany(s => Enum.GetValues<Rank>().Select(r => new Card(s, r)));
}
