namespace Doppelkopf.Core.Cards;

public readonly record struct Card(Suit Suit, Rank Rank)
{
  public string Id => $"{Suit.Symbol()}-{Rank.Display()}";
}
