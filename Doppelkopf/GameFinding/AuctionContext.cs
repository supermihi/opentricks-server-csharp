using Doppelkopf.Configuration;

namespace Doppelkopf.GameFinding;

public sealed record AuctionContext(ByPlayer<bool> NeedsCompulsorySolo, GameModes Modes);