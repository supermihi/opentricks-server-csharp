namespace Doppelkopf.API.Views;

public record TrickTakingView(
  ContractView Contract,
  TrickView? CurrentTrick,
  TrickView? PreviousTrick,
  IReadOnlyList<BidView> Bids);
