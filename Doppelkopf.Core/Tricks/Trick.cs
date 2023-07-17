using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Tricks;

public sealed record Trick(InTurns<Card> Cards, int Index, int Remaining)
{
  public Trick(Player leader, int index, int Remaining) : this(new InTurns<Card>(leader), index, Remaining)
  { }

  public Player Leader => Cards.Start;

  public Trick AddCard(Card card)
  {
    return new Trick(Cards: Cards.Add(card), Index, Remaining);
  }
}
