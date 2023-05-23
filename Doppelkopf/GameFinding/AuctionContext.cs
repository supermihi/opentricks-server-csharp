using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Contracts;

namespace Doppelkopf.GameFinding;

public sealed record AuctionContext(
  ByPlayer<bool> NeedsCompulsorySolo,
  AvailableContracts Contracts,
  ByPlayer<IImmutableList<Card>> Cards
);
