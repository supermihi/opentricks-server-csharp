using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Scoring;

namespace Doppelkopf.Core.Games;


public sealed record PlayerAction(PlayCardAction? PlayCard = null, DeclareAction? DeclareOrHealthy = null, BidAction? Bid = null)
{
  public static PlayerAction Card(Card card) => new(PlayCard: new(card));
  public static class Declare
  {
    public static PlayerAction Healthy() => new(DeclareOrHealthy: new DeclareAction(null));
    public static PlayerAction Hold(string holdId) => new(DeclareOrHealthy: new DeclareAction(holdId));
  }
}

public sealed record PlayCardAction(Card Card);
public sealed record DeclareAction(string? HoldId);
public sealed record BidAction(Bid Bid);

