namespace Doppelkopf.API;

public sealed record AuctionState(ByPlayerState<PlayerAuctionState> State);

public sealed record PlayerAuctionState(bool? IsReserved, bool? HasDeclared);
