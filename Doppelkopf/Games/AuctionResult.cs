using Doppelkopf.Contracts;

namespace Doppelkopf.Games;

public sealed record AuctionResult(IContract Contract, PartyData PartyData);