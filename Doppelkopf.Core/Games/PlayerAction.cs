using Doppelkopf.Core.Auctions;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Scoring;

namespace Doppelkopf.Core.Games;


public sealed record PlayerAction(Card? PlayedCard = null, Declaration? Declaration = null, Bid? Bid = null)
{
  public static class Declare
  {
    public static PlayerAction Fine() => new(Declaration: Declaration.Fine);
    public static PlayerAction Hold(string holdId) => new(Declaration: Declaration.FromHold(holdId));
  }

  public static implicit operator PlayerAction(Declaration d) => new(Declaration: d);
  public static implicit operator PlayerAction(Card c) => new(PlayedCard: c);
}


