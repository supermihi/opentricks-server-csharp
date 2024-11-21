namespace Doppelkopf.Core.Cards;
public static class CardUtils
{

  public static IEnumerable<Card> Jacks => Suits.InOrder.Select(s => new Card(s, Rank.Jack));
  public static IEnumerable<Card> Queens => Suits.InOrder.Select(s => new Card(s, Rank.Queen));
}
