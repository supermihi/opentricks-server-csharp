using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Contracts;

namespace Doppelkopf.Games;

public sealed record AuctionContext(ByPlayer<bool> NeedsCompulsorySolo,
  AvailableContracts Contracts,
  ByPlayer<IImmutableList<Card>> Cards);
