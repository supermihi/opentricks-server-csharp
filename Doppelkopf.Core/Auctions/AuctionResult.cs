namespace Doppelkopf.Core.Auctions;

public sealed record AuctionResult(IDeclarableContract? Contract, Player? Declarer, bool? IsCompulsorySolo);
