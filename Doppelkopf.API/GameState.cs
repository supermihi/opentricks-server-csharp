namespace Doppelkopf.API;



public sealed record GameState(ByPlayerState<IReadOnlyList<string>> Cards, AuctionState? Auction,
  string? Contract, JsonTricks? Tricks);
