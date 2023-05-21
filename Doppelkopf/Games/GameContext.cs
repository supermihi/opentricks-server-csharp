using Doppelkopf.Configuration;
using Doppelkopf.GameFinding;

namespace Doppelkopf.Games;

public sealed record GameContext(ByPlayer<bool> NeedsCompulsorySolo, GameModes Modes)
{
  public AuctionContext AuctionContext => new(NeedsCompulsorySolo, Modes);
}