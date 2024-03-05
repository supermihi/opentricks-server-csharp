using Doppelkopf.Core.Contracts;

namespace Doppelkopf.Core.Auctions;

public sealed record AuctionResult(IHold? Hold, Player? Declarer, bool? IsCompulsorySolo);
