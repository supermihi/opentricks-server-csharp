using Doppelkopf.Contracts;
using Doppelkopf.Sessions;

namespace Doppelkopf.Games;

public record FinishedGame(
  IContract Contract,
  PartyData Parties,
  ByPlayer<Seat> Players,
  Seat? CompulsorySoloist,
  ByPlayer<int> Score
);
