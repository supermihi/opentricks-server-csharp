using Doppelkopf.Core.Contracts;

namespace Doppelkopf.Core.Auctions;

public sealed record AuctionResult(IContract Contract, Player? Declarer, bool? IsCompulsorySolo);
