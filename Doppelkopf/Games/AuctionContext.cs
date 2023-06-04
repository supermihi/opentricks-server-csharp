using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Contracts;
using Doppelkopf.Utils;

namespace Doppelkopf.Games;

public sealed record AuctionContext(ByPlayer<bool> NeedsCompulsorySolo,
  AvailableContracts Contracts,
  ByPlayer<IImmutableList<Card>> Cards);
