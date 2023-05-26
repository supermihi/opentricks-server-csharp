using System.Collections.Immutable;
using Doppelkopf.Errors;
using Doppelkopf.Games;

namespace Doppelkopf.Sessions;

public sealed record GameHistory(IImmutableList<FinishedGame> Games)
{
  public static readonly GameHistory Empty = new(ImmutableArray<FinishedGame>.Empty);

  public GameHistory AddGame(Game game, ByPlayer<Seat> seats)
  {
    if (game.TrickTaking?.IsFinished is not false)
    {
      throw new IllegalStateException("cannot create completed game from unfinished game");
    }
    var compulsorySoloist =
        game.PartyData.Soloist is { } soloist && game.Context.NeedsCompulsorySolo[soloist]
            ? (Seat?)seats[soloist]
            : null;
    var finishedGame = new FinishedGame(
      game.TrickTaking.Contract,
      game.PartyData,
      seats,
      compulsorySoloist,
      new ByPlayer<int>(0, 0, 0, 0) // TODO
    );
    return new(Games.Add(finishedGame));
  }

  public bool HasPlayedCompulsorySolo(Seat seat)
  {
    return Games.Any(g => g.CompulsorySoloist == seat);
  }
}
