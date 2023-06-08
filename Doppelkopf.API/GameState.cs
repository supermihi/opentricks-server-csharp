namespace Doppelkopf.API;



public sealed record GameState(IReadOnlyList<IReadOnlyList<string>> Cards, AuctionState? Auction,
  string? Contract, JsonTricks? Tricks);
