namespace Doppelkopf.API;

public record TrickTakingView(
  ContractView Contract,
  TrickView? CurrentTrick,
  TrickView? PreviousTrick,
  IReadOnlyList<BidView> Bids);
