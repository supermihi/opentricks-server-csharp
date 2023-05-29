using System.Collections.Immutable;
using Doppelkopf.Errors;
using Doppelkopf.Games;

namespace Doppelkopf.Sessions;

public sealed record Finishedgames(IImmutableList<CompleteGame> Games)
{
  public static readonly Finishedgames Empty = new(ImmutableArray<CompleteGame>.Empty);

  public Finishedgames AddGame(Game game, ByPlayer<Seat> seats)
  {
    if (game.TrickTaking?.IsFinished is not false)
    {
      throw new IllegalStateException("cannot create completed game from unfinished game");
    }
    var finishedGame = new CompleteGame(game, seats, new ByPlayer<int>(0, 0, 0, 0));
    return new(Games.Add(finishedGame));
  }

  public bool HasPlayedCompulsorySolo(Seat seat)
  {
    return Games.Any(g => g.CompulsorySoloist == seat);
  }
}
