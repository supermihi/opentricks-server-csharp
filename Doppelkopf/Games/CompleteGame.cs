using Doppelkopf.Sessions;
using Doppelkopf.Utils;

namespace Doppelkopf.Games;

public record CompleteGame(Game Game,
  ByPlayer<Seat> Players,
  ByPlayer<int> Score)
{
  public Seat? CompulsorySoloist =>
      Game.PartyData.Soloist is { } soloist && Game.Context.NeedsCompulsorySolo[soloist]
          ? Players[soloist]
          : null;
}
