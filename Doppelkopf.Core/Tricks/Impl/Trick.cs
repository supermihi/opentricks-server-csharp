using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Tricks.Impl;

public sealed record Trick(InTurns<Card> Cards, Player? Winner) : ITrick
{
  public Trick(Player leader) : this(new InTurns<Card>(leader), null)
  { }

  public Player Leader => Cards.Start;
  public bool IsComplete => Winner.HasValue;

  public Trick AddCard(Card card)
  {
    return this with { Cards = Cards.Add(card) };
  }
}
