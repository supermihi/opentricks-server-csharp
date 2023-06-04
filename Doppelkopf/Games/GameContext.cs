using Doppelkopf.Contracts;
using Doppelkopf.Tricks;
using Doppelkopf.Utils;

namespace Doppelkopf.Games;

public sealed record GameContext(ByPlayer<bool> NeedsCompulsorySolo,
  AvailableContracts Contracts,
  TrickConfiguration TrickConfiguration);