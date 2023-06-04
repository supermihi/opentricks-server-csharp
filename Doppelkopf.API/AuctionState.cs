namespace Doppelkopf.API;

public sealed record AuctionState(ByPlayerState<bool?> Reservations, ByPlayerState<string?> Declarations);
